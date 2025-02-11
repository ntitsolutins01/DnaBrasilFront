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
/// Controle de Encaminhamento
/// </summary>
public class EncaminhamentoController : BaseController
{
    #region Constructor
    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="app">configurações de urls do sistema</param>
    /// <param name="host">informações da aplicação em execução</param>
    public EncaminhamentoController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion 

    #region Main Methods
    /// <summary>
    /// Listagem de Encaminhamento
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">lista de filtros selecionados para pesquisa de alunos</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Encaminhamento, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetEncaminhamentosAll();

        return View(new EncaminhamentoModel() { Encaminhamentos = response });
    }

    /// <summary>
    /// Tela para Inclusão de Encaminhamento
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Encaminhamento, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
            var tipolaudo = new SelectList(ApiClientFactory.Instance.GetTiposLaudoAll(), "Id", "Nome");



            return View(new EncaminhamentoModel()
            {
                ListEstados = estados,
                ListTiposLaudos = tipolaudo
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Inclusão de Encaminhamento
    /// </summary>
    /// <param name="collection">coleção de dados para Inclusao de Encaminhamento</param>
    /// <returns>retorna mensagem de Inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Encaminhamento, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new EncaminhamentoModel.CreateUpdateEncaminhamentoCommand
            {
                TipoLaudoId = Convert.ToInt32(collection["ddlTipoLaudo"].ToString()),
                Nome = collection["nome"].ToString(),
                Parametro = collection["parametro"].ToString(),
                Descricao = collection["descricao"].ToString(),
			};

            foreach (var file in collection.Files)
            {
                if (file.Length <= 0) continue;

                using (var ms = new MemoryStream())
                {
                    file.CopyToAsync(ms);
                    var byteIMage = ms.ToArray();
                    command.ByteImage = byteIMage;
                }
            }

            await ApiClientFactory.Instance.CreateEncaminhamento(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Alteração de Encaminhamento
    /// </summary>
    /// <param name="id">identificador do Encaminhamento</param>
    /// <param name="collection">coleção de dados para alteração de Encaminhamento</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Encaminhamento, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new EncaminhamentoModel.CreateUpdateEncaminhamentoCommand
            {
	            Id = Convert.ToInt32(collection["editEncaminhamentoId"]),
				Nome = collection["nome"].ToString(),
				Parametro = collection["parametro"].ToString(),
				Descricao = collection["descricao"].ToString(),
				Status = collection["editStatus"].ToString() == "" ? false : true

			};

            foreach (var file in collection.Files)
            {
	            if (file.Length <= 0) continue;

	            using var ms = new MemoryStream();
	            await file.CopyToAsync(ms);
	            var byteIMage = ms.ToArray();
	            command.ByteImage = byteIMage;
            }

			await ApiClientFactory.Instance.UpdateEncaminhamento(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão de Encaminhamento
    /// </summary>
    /// <param name="id">identificador do Encaminhamento</param>
    /// <param name="collection">coleção de dados para exclusão de Encaminhamento</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Encaminhamento, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteEncaminhamento(id);
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
    /// Busca Encaminhamento por Id
    /// </summary>
    /// <param name="id">Identificador de Encaminhamneto</param>
    /// <returns>Retorna a um Encaminhamneto</returns>
    public Task<EncaminhamentoDto> GetEncaminhamentoById(int id)
    {
        var result = ApiClientFactory.Instance.GetEncaminhamentoById(id);

        return Task.FromResult(result);
    }
    #endregion
}