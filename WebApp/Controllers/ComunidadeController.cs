using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Authorization;
using WebApp.Configuration;
using WebApp.Enumerators;
using WebApp.Identity;
using WebApp.Utility;

namespace WebApp.Controllers;

public class ComunidadeController : BaseController
{
    #region Constructor
    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="app">configurações de urls do sistema</param>
    /// <param name="host">informações da aplicação em execução</param>
    public ComunidadeController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Crud Methods

    /// <summary>
    /// Listagem de Comunidade
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">lista de filtros selecionados para pesquisa de Comunidades</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Comunidade, Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        //var response = ApiClientFactory.Instance.GetComunidadesAll();

        return View();//new ComunidadeModel() { Comunidades = response }
    }

    /// <summary>
    /// Tela para inclusão de Comunidade
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Comunidade, Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            //var metricas = new SelectList(ApiClientFactory.Instance.GetComunidadesAll(), "Id", "Nome");

            return View();//new ComunidadeModel() { ListComunidades = metricas }
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    #endregion

    ////[ClaimsAuthorize("Usuario", "Incluir")]
    //[HttpPost]
    //public async Task<ActionResult> Create(IFormCollection collection)
    //{
    //    try
    //    {
    //        var command = new ComunidadeModel.CreateUpdateComunidadeCommand
    //        {
    //            Classificacao = collection["classificacao"].ToString(),
    //            Idade = Convert.ToInt32(collection["idade"].ToString()),
    //            ValorInicial = Convert.ToDecimal(collection["valorInicial"].ToString()),
    //            ValorFinal = Convert.ToDecimal(collection["valorFinal"].ToString()),
    //            Sexo = collection["ddlSexo"].ToString()
    //        };

    //        await ApiClientFactory.Instance.CreateComunidade(command);

    //        return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
    //    }
    //    catch (Exception e)
    //    {
    //        return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
    //    }
    //}

    ////[ClaimsAuthorize("Usuario", "Alterar")]
    //public async Task<ActionResult> Edit(IFormCollection collection)
    //{
    //    try
    //    {
    //        var command = new ComunidadeModel.CreateUpdateComunidadeCommand
    //        {
    //            Id = Convert.ToInt32(collection["editComunidadeId"]),
    //            Classificacao = collection["classificacao"].ToString(),
    //            Idade = Convert.ToInt32(collection["idade"].ToString()),
    //            ValorInicial = Convert.ToDecimal(collection["valorInicial"].ToString()),
    //            ValorFinal = Convert.ToDecimal(collection["valorFinal"].ToString()),
    //            Status = collection["editStatus"].ToString() == "" ? false : true
    //        };

    //        await ApiClientFactory.Instance.UpdateComunidade(command.Id, command);

    //        return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
    //    }
    //    catch (Exception e)
    //    {
    //        return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
    //    }
    //}

    ////[ClaimsAuthorize("Usuario", "Excluir")]
    //public ActionResult Delete(int id)
    //{
    //    try
    //    {
    //        ApiClientFactory.Instance.DeleteComunidade(id);
    //        return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
    //    }
    //    catch
    //    {
    //        return RedirectToAction(nameof(Index));
    //    }
    //}

    //public Task<ComunidadeDto> GetComunidadeById(int id)
    //{
    //    var result = ApiClientFactory.Instance.GetComunidadeById(id);

    //    return Task.FromResult(result);
    //}
}