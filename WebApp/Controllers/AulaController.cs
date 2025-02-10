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

namespace WebApp.Controllers;

public class AulaController : BaseController
{
    #region Constructor
    private readonly IOptions<UrlSettings> _appSettings;
    private readonly IWebHostEnvironment _host;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="app">configurações de urls do sistema</param>
    /// <param name="host">informações da aplicação em execução</param>
    public AulaController(IOptions<UrlSettings> appSettings, IWebHostEnvironment host)
    {
        _appSettings = appSettings;
        _host = host;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Crud Methods
    /// <summary>
    /// Listagem de Aula
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">lista de filtros selecionados para pesquisa de alunos</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Aula, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetAulasAll();

        return View(new AulaModel() { Aulas = response });
    }

    /// <summary>
    /// Tela para inclusão de Aula
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
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
    /// Ação de inclusão do Aula
    /// </summary>
    /// <param name="collection">coleção de dados para inclusao de Aula</param>
    /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Aula, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
	        var command = new AulaModel.CreateUpdateAulaCommand
	        {
		        CargaHoraria = Convert.ToInt32(collection["cargaHoraria"].ToString()),
		        ProfessorId = Convert.ToInt32(collection["ddlProfessor"].ToString()),
		        ModuloEadId = Convert.ToInt32(collection["ddlModuloEad"].ToString()),
		        Titulo = collection["titulo"].ToString(),
		        Descricao = collection["descricao"].ToString(),
		        Video = collection["video"].ToString()
	        };

            string? filePath;
            string? fileName;
            string extension = ".jpg";
            string newFileName = Path.ChangeExtension(
	            Guid.NewGuid().ToString(),
	            extension
            );

			foreach (var file in collection.Files)
            {
                if (file.Length <= 0) continue;
                fileName = Path.GetFileName(collection.Files[0].FileName);
                filePath = Path.Combine(_host.WebRootPath, $"Aulas\\{newFileName}");

                if (!Directory.Exists(Path.Combine(_host.WebRootPath, $"Aulas")))
                    Directory.CreateDirectory(Path.Combine(_host.WebRootPath, $"Aulas"));

				command.Material = filePath;
				command.NomeMaterial = fileName;

                await using Stream fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
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
    /// Ação de alteração do Aula
    /// </summary>
    /// <param name="id">identificador do Aula</param>
    /// <param name="collection">coleção de dados para alteração de Aula</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Aula, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new AulaModel.CreateUpdateAulaCommand
            {
                Id = Convert.ToInt32(collection["editAulaId"]),
                CargaHoraria = Convert.ToInt32(collection["cargaHoraria"].ToString()),
                Titulo = collection["nome"].ToString(),
                Descricao = collection["descricao"].ToString(),
                Video = collection["video"].ToString(),
                Status = collection["editStatus"].ToString() == "" ? false : true,
                ProfessorId = Convert.ToInt32(collection["ddlProfessor"].ToString())
            };

            foreach (var file in collection.Files)
            {
                if (file.Length <= 0) continue;

                var currentAula = ApiClientFactory.Instance.GetAulaById(command.Id);

                if (!string.IsNullOrEmpty(currentAula.Material) && System.IO.File.Exists(currentAula.Material))
                {
                    System.IO.File.Delete(currentAula.Material);
                }

                string extension = ".jpg";
                string newFileName = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(_host.WebRootPath, $"Aulas\\{newFileName}");

                if (!Directory.Exists(Path.Combine(_host.WebRootPath, "Aulas")))
                    Directory.CreateDirectory(Path.Combine(_host.WebRootPath, "Aulas"));

                command.Material = filePath;
                command.NomeMaterial = fileName;

                using Stream fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
            }

            if (!collection.Files.Any())
            {
                var currentAula = ApiClientFactory.Instance.GetAulaById(command.Id);
                command.Material = currentAula.Material;
                command.NomeMaterial = currentAula.NomeMaterial;
            }

            await ApiClientFactory.Instance.UpdateAula(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de exclusão do Aula
    /// </summary>
    /// <param name="id">identificador do Aula</param>
    /// <param name="collection">coleção de dados para exclusão de Aula</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Aula, Identity.Claim.Excluir)]
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
    #endregion

    #region Get Methods

    public Task<AulaDto> GetAulaById(int id)
    {
        var result = ApiClientFactory.Instance.GetAulaById(id);
        var professores = new SelectList(ApiClientFactory.Instance.GetUsuarioAll().Where(x => x.Perfil.Id == (int)EnumPerfil.Professor), "Id", "Nome", result.ProfessorId);
        result.ListProfessores = professores;

        return Task.FromResult(result);
    }

    /// <summary>
    /// Método de busca todas as aulas pelo id do módulo ead
    /// </summary>
    /// <param name="id">Id do módulo ead</param>
    /// <returns>Retorna um json com todas as aulas</returns>
    public Task<JsonResult> GetAulasAllByModuloEadId(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("Modulo não informado.");
            var resultLocal = ApiClientFactory.Instance.GetAulasAllByModuloEadId(Convert.ToInt32(id));

            return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Titulo")));

        }
        catch (Exception ex)
        {
            return Task.FromResult(Json(ex.Message));
        }
    }
    #endregion
}
