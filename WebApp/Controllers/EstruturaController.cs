using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApp.Authorization;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;
using Claim = WebApp.Identity.Claim;

namespace WebApp.Controllers;

/// <summary>
/// Controller de Estrutura
/// </summary>
[Authorize(Policy = ModuloAccess.ConfiguracaoSistema)]
public class EstruturaController : BaseController
{
    #region Constructor
    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="app">configurações de urls do sistema</param>
    public EstruturaController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Estrutura
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">lista de filtros selecionados para pesquisa de alunos</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Estrutura, Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetEstruturasAll();

        return View(new EstruturaModel() { Estruturas = response });
    }

    /// <summary>
    /// Tela para inclusão de Estrutura
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Estrutura, Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");

            return View(new EstruturaModel()
            {
                ListEstados = estados
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
        }
    }

    /// <summary>
    /// Ação de Inclusão do Estrutura
    /// </summary>
    /// <param name="collection">coleção de dados para Inclusao de Estrutura</param>
    /// <returns>retorna mensagem de Inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Estrutura, Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new EstruturaModel.CreateUpdateEstruturaCommand
            {
                LocalidadeId = Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                Nome = collection["nome"].ToString(),
                Descricao = collection["descricao"].ToString(),
            };

            await ApiClientFactory.Instance.CreateEstrutura(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Alteração do Estrutura
    /// </summary>
    /// <param name="id">identificador do Estrutura</param>
    /// <param name="collection">coleção de dados para alteração de Estrutura</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Estrutura, Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new EstruturaModel.CreateUpdateEstruturaCommand
            {
                Id = Convert.ToInt32(collection["editEstruturaId"]),
                LocalidadeId = Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                Nome = collection["nome"].ToString(),
                Descricao = collection["descricao"].ToString(),
                Status = collection["editStatus"].ToString() == "" ? false : true,
            };

            await ApiClientFactory.Instance.UpdateEstrutura(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão do Estrutura
    /// </summary>
    /// <param name="id">Identificador do Estrutura</param>
    /// <param name="collection">coleção de dados para Exclusão de Estrutura</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Estrutura, Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            //if (ApiClientFactory.Instance.GetEstruturaById(id).Any())
            //{
            //	return RedirectToAction(nameof(Index), new { EstruturaId = id, notify = (int)EnumNotify.Error, message = "O Estrutura não pode ser excluído pois existem presenças registradas para o mesmo." });
            //}
            ApiClientFactory.Instance.DeleteEstrutura(id);
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
    /// Busca Estrutura por Id
    /// </summary>
    /// <param name="id">Identificador de Estrutura</param>
    /// <returns>Retorna a Estrutura</returns>
    public Task<EstruturaDto> GetEstruturaById(int id)
    {
        //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //var usuario = User.Identity.Name;

        //var usu = ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

        var result = ApiClientFactory.Instance.GetEstruturaById(id);
        var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeByMunicipio(result.Localidade.MunicipioId.ToString()), "Id", "Nome", result.Localidade.Id);
        result.ListLocalidades = localidades;

        return Task.FromResult(result);
    }

    /// <summary>
    /// Busca de Estruturas por localidade
    /// </summary>
    /// <param name="id">Identificador da Localidade</param>
    /// <returns>retorna a lista de Estruturas</returns>
    [ClaimsAuthorize(ClaimType.Estrutura, Claim.Consultar)]
    public Task<JsonResult> GetEstruturasByLocalidade(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("Localidade não informada.");
            var resultLocal = ApiClientFactory.Instance.GetEstruturasByLocalidade(id);

            return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Nome")));

        }
        catch (Exception ex)
        {
            return Task.FromResult(Json(ex));
        }
    }
    #endregion
}
