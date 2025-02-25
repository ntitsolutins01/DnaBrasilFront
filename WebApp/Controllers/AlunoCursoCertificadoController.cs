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

/// <summary>
/// Controle de AlunoCurso
/// </summary>
public class AlunoCursoCertificadoController : BaseController
{

    #region Parametros

    private readonly IOptions<UrlSettings> _appSettings;
    private readonly IWebHostEnvironment _host;

    #endregion

    #region Constructor

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">Configurações de urls do sistema</param>
    /// <param name="host">Informações da aplicação em execução</param>
    public AlunoCursoCertificadoController(IOptions<UrlSettings> appSettings, IWebHostEnvironment host)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        _host = host;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de AlunoCurso
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        var usuario = User.Identity.Name;

        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);

        var aluno = ApiClientFactory.Instance.GetAlunoByEmail(usuario);
        var cursos = ApiClientFactory.Instance.GetCursosByAlunoId(aluno.Id);
        var certificados = ApiClientFactory.Instance.GetCertificadosByAlunoId(aluno.Id);

        return View(new AlunoCursoCertificadoModel()
        {
            AlunoId = aluno.Id,
            Cursos = cursos,
            Certificados = certificados
        });
    }

    /// <summary>
    /// Tela para Inclusão de AlunoCurso
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Incluir)]
    public async Task<ActionResult> CreateAlunoCurso(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
            var cursos = new SelectList(ApiClientFactory.Instance.GetCursosAll(), "Id", "Titulo");

            return View(new AlunoCursoCertificadoModel()
            {
                ListEstados = estados,
                ListCursos = cursos
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
        }
    }

    /// <summary>
    /// Ação de Inclusão do AlunoCurso
    /// </summary>
    /// <param name="collection">Coleção de dados para inclusao de AlunoCurso</param>
    /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> CreateAlunoCurso(IFormCollection collection)
    {
        try
        {
            var command = new AlunoCursoCertificadoModel.CreateUpdateAlunoCursoCommand
            {
                AlunoId = Convert.ToInt32(collection["ddlAluno"].ToString()),
                CursoId = Convert.ToInt32(collection["ddlCurso"].ToString())
            };

            await ApiClientFactory.Instance.CreateAlunoCurso(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Tela para Inclusão de AlunoCertificado
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Incluir)]
    public async Task<ActionResult> CreateAlunoCertificado(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
            var certificados = new SelectList(ApiClientFactory.Instance.GetCertificadosAll(), "Id", "NomeImagemFrente");

            return View(new AlunoCursoCertificadoModel()
            {
                ListEstados = estados,
                ListCertificados = certificados
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
        }
    }

    /// <summary>
    /// Ação de Inclusão do AlunoCertificado
    /// </summary>
    /// <param name="collection">Coleção de dados para inclusao de AlunoCertificado</param>
    /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> CreateAlunoCertificado(IFormCollection collection)
    {
        try
        {
            var command = new AlunoCursoCertificadoModel.CreateUpdateAlunoCertificadoCommand
            {
                AlunoId = Convert.ToInt32(collection["ddlAluno"].ToString()),
                CertificadoId = Convert.ToInt32(collection["ddlCertificado"].ToString())
            };

            await ApiClientFactory.Instance.CreateAlunoCertificado(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    ///// <summary>
    ///// Ação de Alteração do AlunoCurso
    ///// </summary>
    ///// <param name="id">Identificador do AlunoCurso</param>
    ///// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    //[ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Alterar)]
    //public async Task<ActionResult> Edit(IFormCollection collection)
    //{
    //    try
    //    {
    //        var command = new AlunoCursoModel.CreateUpdateAlunoCursoCommand
    //        {
    //            Id = Convert.ToInt32(collection["editAlunoCursoId"]),
    //            CargaHoraria = Convert.ToInt32(collection["cargaHoraria"].ToString()),
    //            Titulo = collection["nome"].ToString(),
    //            Descricao = collection["descricao"].ToString(),
    //            Video = collection["video"].ToString(),
    //            Status = collection["editStatus"].ToString() == "" ? false : true,
    //            ProfessorId = Convert.ToInt32(collection["ddlProfessor"].ToString())
    //        };

    //        foreach (var file in collection.Files)
    //        {
    //            if (file.Length <= 0) continue;

    //            var currentAlunoCurso = ApiClientFactory.Instance.GetAlunoCursoById(command.Id);

    //            if (!string.IsNullOrEmpty(currentAlunoCurso.Material) && System.IO.File.Exists(currentAlunoCurso.Material))
    //            {
    //                System.IO.File.Delete(currentAlunoCurso.Material);
    //            }

    //            string extension = ".jpg";
    //            string newFileName = Path.ChangeExtension(Guid.NewGuid().ToString(), extension);
    //            string fileName = Path.GetFileName(file.FileName);
    //            string filePath = Path.Combine(_host.WebRootPath, $"AlunosCursos\\{newFileName}");

    //            if (!Directory.Exists(Path.Combine(_host.WebRootPath, "AlunosCursos")))
    //                Directory.CreateDirectory(Path.Combine(_host.WebRootPath, "AlunosCursos"));

    //            command.Material = filePath;
    //            command.NomeMaterial = fileName;

    //            using Stream fileStream = new FileStream(filePath, FileMode.Create);
    //            await file.CopyToAsync(fileStream);
    //        }

    //        if (!collection.Files.Any())
    //        {
    //            var currentAlunoCurso = ApiClientFactory.Instance.GetAlunoCursoById(command.Id);
    //            command.Material = currentAlunoCurso.Material;
    //            command.NomeMaterial = currentAlunoCurso.NomeMaterial;
    //        }

    //        await ApiClientFactory.Instance.UpdateAlunoCurso(command.Id, command);

    //        return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
    //    }
    //    catch (Exception e)
    //    {
    //        return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
    //    }
    //}

    /// <summary>
    /// Ação de Exclusão do AlunoCurso
    /// </summary>
    /// <param name="id">Identificador do AlunoCurso</param>
    /// <param name="collection">Coleção de dados para exclusão de AlunoCurso</param>
    [ClaimsAuthorize(ClaimType.Aluno, Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteAlunoCurso(id);
            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
        }
        catch
        {
            return RedirectToAction(nameof(Index));
        }
    }
    #endregion

    #region Get Methods

    /// <summary>
    /// Busca de AlunoCurso por Id
    /// </summary>
    /// <param name="id">Identificador de AlunoCurso</param>
    /// <returns>Retorna a AlunoCurso</returns>
    public Task<AlunoCursoDto> GetAlunoCursoById(int id)
    {
        var result = ApiClientFactory.Instance.GetAlunoCursoById(id);

        return Task.FromResult(result);
    }

    #endregion
}
