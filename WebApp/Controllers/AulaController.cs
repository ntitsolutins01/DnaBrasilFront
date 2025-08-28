using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Authorization;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;
using Path = System.IO.Path;

namespace WebApp.Controllers;

/// <summary>
/// Controle de Aula
/// </summary>
public class AulaController : BaseController
{

    #region Parametros

    private readonly IOptions<UrlSettings> _appSettings;
    private readonly IWebHostEnvironment _host;
    private readonly ILog _logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">Configurações de urls do sistema</param>
    /// <param name="host">Informações da aplicação em execução</param>
    /// <param name="logger">Log de mensagens da aplicação</param>
    /// <param name="settings">Configurações parametrizadas do sistema</param>
    public AulaController(IOptions<UrlSettings> appSettings, IWebHostEnvironment host,
        ILog logger)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        _host = host;
        _logger = logger;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Aula
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Aula, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetAulasAll();

        return View(new AulaModel() { Aulas = response });
    }

    /// <summary>
    /// Tela para Inclusão de Aula
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Aula, Identity.Claim.Incluir)]
    public async Task<ActionResult> Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            //criar metodo que busca Usuarios por PerfilId
            var usuario = ApiClientFactory.Instance.GetUsuarioAll().Where(x => x.Perfil.Id == (int)EnumPerfil.Professor);
            var professores = new SelectList(usuario, "Id", "Nome");
            var tipoCurso = new SelectList(ApiClientFactory.Instance.GetTipoCursosAll(), "Id", "Nome");

            return View(new AulaModel()
            {
                ListProfessores = professores,
                ListTipoCursos = tipoCurso
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
        }
    }

    /// <summary>
    /// Ação de Inclusão do Aula
    /// </summary>
    /// <param name="collection">Coleção de dados para inclusao de Aula</param>
    /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Aula, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new AulaModel.CreateUpdateAulaCommand
            {
                ProfessorId = Convert.ToInt32(collection["ddlProfessor"].ToString()),
                ModuloEadId = Convert.ToInt32(collection["ddlModuloEad"].ToString()),
                Titulo = collection["titulo"].ToString(),
                Descricao = collection["descricao"].ToString(),
            };

            string aulasPath = Path.Combine(_host.WebRootPath, "Aulas");
            if (!Directory.Exists(aulasPath))
            {
                Directory.CreateDirectory(aulasPath);
            }

            string? filePathMaterial = null;
            string? fileNameMaterial = null;
            string? filePathVideo = null;
            string? fileNameVideo = null;

            foreach (var file in collection.Files)
            {
                if (file.Length <= 0) continue;

                string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                switch (extension)
                {
                    case ".jpg":
                    case ".png":
                        {
                            string newFileNameMaterial = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
                            filePathMaterial = Path.Combine(aulasPath, newFileNameMaterial);
                            fileNameMaterial = Path.GetFileName(file.FileName);

                            command.Material = filePathMaterial;
                            command.NomeMaterial = fileNameMaterial;

                            using (var fileStream = new FileStream(filePathMaterial, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            break;
                        }
                    case ".mp4":
                    case ".avi":
                        {
                            string newFileNameVideo = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
                            filePathVideo = Path.Combine(aulasPath, newFileNameVideo);
                            fileNameVideo = Path.GetFileName(file.FileName);

                            command.Video = filePathVideo;
                            command.NomeVideo = fileNameVideo;

                            using (var fileStream = new FileStream(filePathVideo, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            break;
                        }
                    default:
                        return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao realizar Upload. Somente arquivos MP4 e AVI são permitidos." });
                }
            }

            await ApiClientFactory.Instance.CreateAula(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Alteração do Aula
    /// </summary>
    /// <param name="id">Identificador do Aula</param>
    /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Aula, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var uriMaterial = collection["material"];//new Uri(collection["material"].ToString()); //transforma em Uri String pra salvar no banco 
            var nomeMaterial = collection["nomeMaterial"].ToString(); //Path.GetFileName(uriMaterial.LocalPath);
            var uriVideo = collection["video"]; //new Uri(collection["video"].ToString());
            var nomeVideo = collection["nomeVideo"].ToString(); //Path.GetFileName(uriVideo.LocalPath);

            AulaModel.CreateUpdateAulaCommand command;
            command = new AulaModel.CreateUpdateAulaCommand
            {
                Id = Convert.ToInt32(collection["editAulaId"]),
                Titulo = collection["nome"].ToString(),
                Descricao = collection["descricao"].ToString(),
                Status = collection["editStatus"].ToString() == "" ? false : true,
                ProfessorId = Convert.ToInt32(collection["ddlProfessor"].ToString()),
                Ordem = Convert.ToInt32(collection["ordem"].ToString()),
                Video = uriVideo,
                NomeVideo = nomeVideo,
                Material = uriMaterial,
                NomeMaterial = nomeMaterial
            };

            //string aulasPath = Path.Combine(_host.WebRootPath, "Aulas");
            //if (!Directory.Exists(aulasPath))
            //{
            //    Directory.CreateDirectory(aulasPath);
            //}

            //string? filePathMaterial = null;
            //string? fileNameMaterial = null;
            //string? filePathVideo = null;
            //string? fileNameVideo = null;

            //foreach (var file in collection.Files)
            //{
            //    if (file.Length <= 0) continue;

            //    string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            //    if (extension == ".jpg" || extension == ".png")
            //    {
            //        string newFileNameMaterial = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
            //        filePathMaterial = Path.Combine(aulasPath, newFileNameMaterial);
            //        fileNameMaterial = Path.GetFileName(file.FileName);

            //        command.Material = filePathMaterial;
            //        command.NomeMaterial = fileNameMaterial;

            //        using (var fileStream = new FileStream(filePathMaterial, FileMode.Create))
            //        {
            //            await file.CopyToAsync(fileStream);
            //        }
            //    }
            //    else if (extension == ".mp4" || extension == ".avi")
            //    {
            //        string newFileNameVideo = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
            //        filePathVideo = Path.Combine(aulasPath, newFileNameVideo);
            //        fileNameVideo = Path.GetFileName(file.FileName);

            //        command.Video = filePathVideo;
            //        command.NomeVideo = fileNameVideo;

            //        using (var fileStream = new FileStream(filePathVideo, FileMode.Create))
            //        {
            //            await file.CopyToAsync(fileStream);
            //        }
            //    }
            //}

            await ApiClientFactory.Instance.UpdateAula(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão do Aula
    /// </summary>
    /// <param name="id">Identificador do Aula</param>
    /// <param name="collection">Coleção de dados para exclusão de Aula</param>
    [ClaimsAuthorize(ClaimType.Aluno, Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            var material = ApiClientFactory.Instance.GetAulaById(id).Material!;

            if (material != null)
                System.IO.File.Delete(material);

            ApiClientFactory.Instance.DeleteAula(id);
            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
        }
        catch
        {
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Ação de Alteração do Aula
    /// </summary>
    /// <param name="id">Identificador do Aula</param>
    /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Aula, Identity.Claim.Alterar)]
    [HttpPost]
    public async Task<IActionResult> Order([FromBody] AulaModel.CreateUpdateAulaCommand model)
    {
        try
        {
            var command = new AulaModel.CreateUpdateAulaCommand
            {
                Id = model.Id,
                Titulo = model.Titulo,
                Status = model.Status,
                ProfessorId = model.ProfessorId,
                Material = model.Material,
                NomeMaterial = model.NomeMaterial,
                Video = model.Video,
                Descricao = model.Descricao,
                Ordem = model.Ordem
            };

            await ApiClientFactory.Instance.UpdateAula(model.Id, command);

            return NoContent();
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }
    #endregion

    #region Get Methods

    /// <summary>
    /// Busca de Aula por Id
    /// </summary>
    /// <param name="id">Identificador de Aula</param>
    /// <returns>Retorna a Aula</returns>
    public Task<AulaDto> GetAulaById(int id, bool? bloob = null)
    {
        var result = ApiClientFactory.Instance.GetAulaById(id);

        if (bloob != null && (bool)bloob)
        {
            result.Material = _appSettings.Value.BloobUrl + result.Material;
            result.Video = _appSettings.Value.BloobUrl + result.Video;
        }


        var professores = result.ProfessorId == null ? null : new SelectList(ApiClientFactory.Instance.GetUsuarioAll().Where(x => x.Perfil.Id == (int)EnumPerfil.Professor), "Id", "Nome", result.ProfessorId);
        result.ListProfessores = professores;

        return Task.FromResult(result);
    }

    /// <summary>
    /// Método de busca todas as aulas pelo id do módulo ead
    /// </summary>
    /// <param name="id">Id do módulo ead</param>
    /// <returns>Retorna um json com todas as aulas</returns>
    public Task<JsonResult> GetAulasByModuloEadId(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("Modulo não informado.");
            var resultLocal = ApiClientFactory.Instance.GetAulasByModuloEadId(Convert.ToInt32(id));

            return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Titulo")));

        }
        catch (Exception ex)
        {
            return Task.FromResult(Json(ex.Message));
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Ação de Upload de vídeo da auula
    /// </summary>
    /// <param name="collection">Arquivo de upload realizado</param>
    /// <returns>Retorna mensagem de upload realizado através do parametro notfy e message</returns>
    [HttpPost]
    public async Task<ActionResult> Upload(IFormCollection collection)
    {
        try
        {
            _logger.Info($"Ação de upload de video da Aula - Aula.Upload");

            string filePath = null;

            var aula = ApiClientFactory.Instance.GetAulaById(Convert.ToInt32(collection["aulaId"]));

            var command = new AulaModel.CreateUpdateAulaCommand
            {
                Id = Convert.ToInt32(collection["aulaId"]),
                ProfessorId = aula.ProfessorId,
                Titulo = aula.Titulo
            };

            string aulasPath = Path.Combine(_host.WebRootPath, "Aulas");
            if (!Directory.Exists(aulasPath))
            {
                Directory.CreateDirectory(aulasPath);
            }

            string? filePathMaterial = null;
            string? fileNameMaterial = null;
            string? filePathVideo = null;
            string? fileNameVideo = null;

            foreach (var file in collection.Files)
            {
                if (file.Length <= 0) continue;

                string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                switch (extension)
                {
                    case ".jpg":
                    case ".png":
                        {
                            string newFileNameMaterial = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
                            filePathMaterial = Path.Combine(aulasPath, newFileNameMaterial);
                            fileNameMaterial = Path.GetFileName(file.FileName);

                            command.Material = filePathMaterial;
                            command.NomeMaterial = fileNameMaterial;

                            using (var fileStream = new FileStream(filePathMaterial, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            break;
                        }
                    case ".mp4":
                    case ".avi":
                        {
                            string newFileNameVideo = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
                            filePathVideo = Path.Combine(aulasPath, newFileNameVideo);
                            fileNameVideo = Path.GetFileName(file.FileName);

                            command.Video = filePathVideo;
                            command.NomeVideo = fileNameVideo;

                            using (var fileStream = new FileStream(filePathVideo, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            break;
                        }
                    default:
                        return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao realizar Upload. Somente arquivos MP4 e AVI são permitidos." });
                }
            }
            
            if (aula.Video != null)
            {
                System.IO.File.Delete(aula.Video);
            }


            await ApiClientFactory.Instance.UpdateAula(command.Id, command);

            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Success, message = "Upload realizado com sucesso." });


        }
        catch (Exception e)
        {
            _logger.Error($"Ação de upload de foto do aluno - Aluno.Upload: {e.StackTrace}");
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
        }
    }
    #endregion
}
