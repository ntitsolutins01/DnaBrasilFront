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

public class RankingController : BaseController
{
    #region Parametros

    private readonly IOptions<UrlSettings> _appSettings;

    #endregion

    #region Constructor

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">Configurações de urls do sistema</param>
    /// <param name="host">Informações da aplicação em execução</param>
    public RankingController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
	#endregion

	#region Main Methods

	/// <summary>
	/// Listagem de Ranking
	/// </summary>
	/// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
	/// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
	/// <param name="collection">Lista de filtros selecionados para pesquisa de alunos</param>
	/// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
	[ClaimsAuthorize(ClaimType.Ranking, Claim.Consultar)]
	public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        //var response = ApiClientFactory.Instance.GetRankingsAll();

        return View();//new RankingModel() { Rankings = response }
    }

	/// <summary>
	/// Tela para inclusão de aluno
	/// </summary>
	/// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
	/// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
	/// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
	[ClaimsAuthorize(ClaimType.Ranking, Claim.Incluir)]
	public ActionResult Create(int? crud, int? notify, string message = null)
	{
		try
		{
			SetNotifyMessage(notify, message);
			SetCrudMessage(crud);
			//var metricas = new SelectList(ApiClientFactory.Instance.GetRankingsAll(), "Id", "Nome");

			return View();//new RankingModel() { ListRankings = metricas }
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
	//        var command = new RankingModel.CreateUpdateRankingCommand
	//        {
	//            Classificacao = collection["classificacao"].ToString(),
	//            Idade = Convert.ToInt32(collection["idade"].ToString()),
	//            ValorInicial = Convert.ToDecimal(collection["valorInicial"].ToString()),
	//            ValorFinal = Convert.ToDecimal(collection["valorFinal"].ToString()),
	//            Sexo = collection["ddlSexo"].ToString()
	//        };

	//        await ApiClientFactory.Instance.CreateRanking(command);

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
	//        var command = new RankingModel.CreateUpdateRankingCommand
	//        {
	//            Id = Convert.ToInt32(collection["editRankingId"]),
	//            Classificacao = collection["classificacao"].ToString(),
	//            Idade = Convert.ToInt32(collection["idade"].ToString()),
	//            ValorInicial = Convert.ToDecimal(collection["valorInicial"].ToString()),
	//            ValorFinal = Convert.ToDecimal(collection["valorFinal"].ToString()),
	//            Status = collection["editStatus"].ToString() == "" ? false : true
	//        };

	//        await ApiClientFactory.Instance.UpdateRanking(command.Id, command);

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
	//        ApiClientFactory.Instance.DeleteRanking(id);
	//        return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
	//    }
	//    catch
	//    {
	//        return RedirectToAction(nameof(Index));
	//    }
	//}

	//public Task<RankingDto> GetRankingById(int id)
	//{
	//    var result = ApiClientFactory.Instance.GetRankingById(id);

	//    return Task.FromResult(result);
	//}
}