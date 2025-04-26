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
        var alunosCursos = ApiClientFactory.Instance.GetAlunoCursosByAlunoId(aluno.Id);

        return View(new AlunoCursoCertificadoModel()
        {
            AlunoId = aluno.Id,
            Cursos = cursos,
            Certificados = certificados,
            AlunosCursos = alunosCursos
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
            var command = new AlunoModel.CreateUpdateAlunoCursoCommand
            {
                AlunoId = Convert.ToInt32(collection["ddlAluno"].ToString()),
                CursosId = collection["ddlCurso"].ToString()
            };

            await ApiClientFactory.Instance.CreateAlunoCursos(command);

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
            var command = new AlunoModel.CreateUpdateAlunoCertificadoCommand
            {
                AlunoId = Convert.ToInt32(collection["ddlAluno"].ToString()),
                CertificadosId = collection["ddlCertificado"].ToString()
            };

            await ApiClientFactory.Instance.CreateAlunoCertificados(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Alteração do Curso
    /// </summary>
    /// <param name="model">Modelo de dados para alteração de Curso</param>
    /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UpdateProgresso([FromForm] AlunoCursoCertificadoModel.UpdateProgressoCommand model)
    {
        try
        {
            if (model != null)
            {
                var command = new AlunoCursoCertificadoModel.UpdateProgressoCommand
                {
                    AlunoId = model.AlunoId,
                    CursoId = model.CursoId,
                    Progresso = Convert.ToInt32(model.Progresso)
                };

                await ApiClientFactory.Instance.UpdateAlunoCurso(command.AlunoId, command.CursoId, command);
            }

            return NoContent();
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

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

    /// <summary>
    /// Exibe os detalhes de um curso específico com seus módulos e aulas
    /// </summary>
    /// <param name="id">ID do curso</param>
    /// <param name="aulaId">ID opcional da aula a ser exibida inicialmente</param>
    /// <returns>View com os detalhes do curso</returns>
    [ClaimsAuthorize(ClaimType.Curso, Identity.Claim.Consultar)]
    public IActionResult DetalhesCurso(int id)
    {
        try
        {
            var usuario = User.Identity.Name;

            var aluno = ApiClientFactory.Instance.GetAlunoByEmail(usuario);
            var curso = ApiClientFactory.Instance.GetCursoById(id);

            var alunosCursos = ApiClientFactory.Instance.GetAlunosCursosByCursoId(id);
            var alunoCurso = alunosCursos.FirstOrDefault(ac => Convert.ToInt32(ac.AlunoId) == aluno.Id);

            var modulos = ApiClientFactory.Instance.GetModulosEadAllByCursoId(id);

            var aulas = ApiClientFactory.Instance.GetAulasByCursoId(id);

            var alunosAulas = ApiClientFactory.Instance.GetAlunoAulasByAlunoId(aluno.Id);

            var model = new AlunoCursoCertificadoModel()
            {
                AlunoCurso = alunoCurso,
                Aluno = aluno,
                Curso = curso,
                Modulos = modulos,
                Aulas = aulas,
                AlunosAulas = alunosAulas
            };

            return View(model);

        }
        catch (Exception ex)
        {
            Console.Write(ex.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao abrir a página de detalhes do curso." });
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
