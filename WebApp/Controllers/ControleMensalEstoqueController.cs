using Microsoft.AspNetCore.Authorization;
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
/// Controle Mensal de Estoque
/// </summary>
[Authorize(Policy = ModuloAccess.ConfiguracaoSistemaEad)]
public class ControleMensalEstoqueController : BaseController
{
    #region Constructor
    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">configurações de urls do sistema</param>
    /// <param name="host">Informações da aplicação em execução</param>
    public ControleMensalEstoqueController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion 

    #region Main Methods
    /// <summary>
    /// Listagem de Controle Mensal de Estoque
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">Lista de filtros selecionados para pesquisa de alunos</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.ControleMensalEstoque, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetControlesMensaisEstoqueAll();

        return View(new ControleMensalEstoqueModel() { ControlesMensaisEstoque = response });
    }

    /// <summary>
    /// Tela para Inclusão de Controle Mensal de Estoque
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.ControleMensalEstoque, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var tipoMateriais = new SelectList(ApiClientFactory.Instance.GetTiposMateriaisAll(), "Id", "Nome");

            return View(new ControleMensalEstoqueModel()
            {
                ListTiposMateriais = tipoMateriais
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Inclusão de Controle Mensal de Estoque
    /// </summary>
    /// <param name="collection">Coleção de dados para inclusao de Controle Mensal de Estoque</param>
    /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ControleMensalEstoque, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new ControleMensalEstoqueModel.CreateUpdateControleMensalEstoqueCommand
            {
                MaterialId = Convert.ToInt32(collection["ddlMaterial"].ToString()),
                QtdPrevista = Convert.ToInt32(collection["qtdPrevista"].ToString()),
                DataMesSaida = collection["dataMesSaida"].ToString(),
                TotalSaidas = Convert.ToInt32(collection["totalSaidas"].ToString()),
                TotalEstoque = Convert.ToInt32(collection["totalEstoque"].ToString()),
                QtdMateriaisDanificadosExtraviados = Convert.ToInt32(collection["qtdDanificadoExtraviado"].ToString()),
                JustificativaDanificadosExtraviados = collection["justificativaDanificadoExtraviado"].ToString(),
                DataDanificadosExtraviados = collection["dataDanificadoExtraviado"].ToString()
            };

            await ApiClientFactory.Instance.CreateControleMensalEstoque(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }


    /// <summary>
    /// Ação de Alteração de Controle Mensal de Estoque
    /// </summary>
    /// <param name="id">identificador de Controle Mensal de Estoque</param>
    /// <param name="collection">Coleção de dados para alteração de Controle Mensal e Estoque</param>
    /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ControleMensalEstoque, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new ControleMensalEstoqueModel.CreateUpdateControleMensalEstoqueCommand
            {
                Id = Convert.ToInt32(collection["editControleMensalEstoqueId"]),
                MaterialId = Convert.ToInt32(collection["ddlMaterial"].ToString()),
                QtdPrevista = Convert.ToInt32(collection["qtdPrevista"].ToString()),
                DataMesSaida = collection["dataMesSaida"].ToString(),
                TotalSaidas = Convert.ToInt32(collection["totalSaidas"].ToString()),
                TotalEstoque = Convert.ToInt32(collection["totalEstoque"].ToString()),
                QtdMateriaisDanificadosExtraviados = Convert.ToInt32(collection["qtdDanificadoExtraviado"].ToString()),
                JustificativaDanificadosExtraviados = collection["justificativaDanificadoExtraviado"].ToString(),
                DataDanificadosExtraviados = collection["dataDanificadoExtraviado"].ToString()
            };

            await ApiClientFactory.Instance.UpdateControleMensalEstoque(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão de Controle Mensal de Estoque
    /// </summary>
    /// <param name="id">Identificador do ControleMensalEstoque</param>
    /// <param name="collection">Coleção de dados para exclusão de ControleMensalEstoque</param>
    /// <returns>Retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ControleMensalEstoque, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteControleMensalEstoque(id);
            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Este grupo de material não pode ser excluído pois possui aulas vinculadas a ele." });
        }
    }

    //public Task<JsonResult> GetControlesMensaisEstoqueByMaterialId(string id)
    //{
    //    try
    //    {
    //        if (string.IsNullOrEmpty(id)) throw new Exception("Material não informado.");
    //        var resultLocal = ApiClientFactory.Instance.GetControlesMensaisEstoqueByMaterialId(Convert.ToInt32(id));

    //        return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Descricao")));

    //    }
    //    catch (Exception ex)
    //    {
    //        return Task.FromResult(Json(ex.Message));
    //    }
    //}
    #endregion

    #region Get Methods

    /// <summary>
    /// Busca Controle Mensal de Estoque por Id
    /// </summary>
    /// <param name="id">Identificador de Controle Mensal de Estoque </param>
    /// <returns>Retorna o Controle Mensal de Estoque</returns>
    public Task<ControleMensalEstoqueDto> GetControleMensalEstoqueById(int id)
    {
        var result = ApiClientFactory.Instance.GetControleMensalEstoqueById(id);

        return Task.FromResult(result);
    }
    #endregion
}