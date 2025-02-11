using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
/// Controle de Tipo Curso
/// </summary>
[Authorize(Policy = ModuloAccess.ConfiguracaoSistemaEad)]
public class TipoCursoController : BaseController
{
    #region Constructor
    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="app">configurações de urls do sistema</param>
    /// <param name="host">informações da aplicação em execução</param>
    public TipoCursoController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Tipo Curso
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">lista de filtros selecionados para pesquisa de TipoCursos</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.TipoCurso, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetTipoCursosAll();

        return View(new TipoCursoModel() { TiposCursos = response });
    }

    /// <summary>
    /// Tela para Inclusão de Tipo Curso
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.TipoCurso, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            //var TipoCurso = new SelectList(ApiClientFactory.Instance.GetTipoCursosAll(), "Id", "Nome");

            return View(); //(new TipoCursoModel() {  = TipoCurso });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Inclusao do Tipo Curso
    /// </summary>
    /// <param name="collection">coleção de dados para inclusao de Tipo Curso</param>
    /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.TipoCurso, Identity.Claim.Incluir)]
	[HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new TipoCursoModel.CreateUpdateTipoCursoCommand
            {
                Nome = collection["nome"].ToString()
            };

            await ApiClientFactory.Instance.CreateTipoCurso(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Alteração do Tipo Curso
    /// </summary>
    /// <param name="id">identificador do Tipo Curso</param>
    /// <param name="collection">coleção de dados para alteração de Tipo Curso</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.TipoCurso, Identity.Claim.Alterar)]
	public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new TipoCursoModel.CreateUpdateTipoCursoCommand
            {
                Id = Convert.ToInt32(collection["editTipoCursoId"]),
                Nome = collection["nome"].ToString(),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            await ApiClientFactory.Instance.UpdateTipoCurso(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão do Tipo Curso
    /// </summary>
    /// <param name="id">identificador do Tipo Curso</param>
    /// <param name="collection">coleção de dados para exclusão de Tipo Curso</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
	[ClaimsAuthorize(ClaimType.TipoCurso, Identity.Claim.Excluir)]
	public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteTipoCurso(id);
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
	/// Busca de Tipo de Curdo  por id
	/// </summary>
	/// <param name="id">identificador do Tipo de Curso</param>
	/// <returns>retorna o tipo de curso</returns>
	public Task<TiposCursoDto> GetTipoCursoById(int id)
    {
        var result = ApiClientFactory.Instance.GetTipoCursoById(id);

        return Task.FromResult(result);
    }

    #endregion

}