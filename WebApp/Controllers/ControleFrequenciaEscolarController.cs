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
/// Controle de Frequencia Escolar
/// </summary>
public class ControleFrequenciaEscolarController : BaseController
{
    #region Parametros

    private readonly IOptions<UrlSettings> _appSettings;

    #endregion

    #region Constructor

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">Configurações de urls do sistema</param>
    public ControleFrequenciaEscolarController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Controle Frequencia Escolar
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Laudo, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
        var etapas = new SelectList(ApiClientFactory.Instance.GetEtapasEnsinoAll(), "Id", "Nome");
        var disciplinas = new SelectList(ApiClientFactory.Instance.GetDisciplinasAll(), "Id", "Nome");
        var response = ApiClientFactory.Instance.GetControlesFrequenciasEscolaresAll();

        return View(new ControleFrequenciaEscolarModel()
        {
            ListEstados = estados,
            ListEtapas = etapas,
            ListDisciplinas = disciplinas,
            ControlesFrequenciasEscolares = response
        });
    }

    ///// <summary>
    ///// Tela para Inclusão de Controle Frequencia Escolar
    ///// </summary>
    ///// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    ///// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    ///// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    //[ClaimsAuthorize(ClaimType.Laudo, Identity.Claim.Incluir)]
    //public ActionResult Create(int? crud, int? notify, string message = null)
    //{
    //    try
    //    {
    //        SetNotifyMessage(notify, message);
    //        SetCrudMessage(crud);
    //        var linhaAcao = new SelectList(ApiClientFactory.Instance.GetLinhasAcoesAll(), "Id", "Nome");

    //        return View(new ControleFrequenciaEscolarModel()
    //        {
    //            ListLinhasAcoes = linhaAcao
    //        });
    //    }
    //    catch (Exception e)
    //    {
    //        Console.Write(e.StackTrace);
    //        return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

    //    }
    //}

    ///// <summary>
    ///// Ação de Inclusão da Frequencia Escolar
    ///// </summary>
    ///// <param name="collection">Coleção de dados para inclusao de Controle Frequencia Escolar</param>
    ///// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
    //[ClaimsAuthorize(ClaimType.Laudo, Identity.Claim.Incluir)]
    //[HttpPost]
    //public async Task<ActionResult> Create(IFormCollection collection)
    //{
    //    try
    //    {
    //        var command = new ControleFrequenciaEscolarModel.CreateUpdateControleFrequenciaEscolarCommand
    //        {
    //            Id = Convert.ToInt32(collection["editControleFrequenciaEscolarId"]),
    //            Controle = collection["controle"].ToString(),
    //            AlunoId = Convert.ToInt32(collection["ddlAluno"]),
    //        };

    //        //var possuiControleFrequenciaEscolar = ApiClientFactory.Instance.GetControleFrequenciaEscolarByAlunoIdDisciplinaId(Convert.ToInt32(command.AlunoId), Convert.ToInt32(command.DisciplinaId));

    //        //if (possuiControleFrequenciaEscolar==null)
    //        //{
    //        // return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Warning, message = "Já existe ControleFrequenciaEscolar cadastrada para este aluno na disciplina informada." });
    //        //}

    //        await ApiClientFactory.Instance.CreateControleFrequenciaEscolar(command);

    //        return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
    //    }
    //    catch (Exception e)
    //    {
    //        return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
    //    }
    //}

    /// <summary>
    /// Ação de Exclusão da Frequencia Escolar
    /// </summary>
    /// <param name="id">Identificador da Frequencia Escolar</param>
    /// <returns>Retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Laudo, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteControleFrequenciaEscolar(id);
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
    /// Método de busca de Controle de Frequencia Escolar por id
    /// </summary>
    /// <param name="id">Id do Frequencia Escolar </param>
    /// <returns>Retorna o objeto Controle de Frequencia Escolar</returns>
    [ClaimsAuthorize(ClaimType.Laudo, Identity.Claim.Consultar)]
    public Task<ControleFrequenciaEscolarDto> GetControleFrequenciaEscolarById(int id)
    {
        var result = ApiClientFactory.Instance.GetControleFrequenciaEscolarById(id);

        return Task.FromResult(result);
    }
    #endregion
}