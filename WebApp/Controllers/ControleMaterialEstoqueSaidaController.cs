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
/// Controle de Material de Estoque e Saida
/// </summary> 
[Authorize(Policy = ModuloAccess.ConfiguracaoSistemaEad)]
public class ControleMaterialEstoqueSaidaController : BaseController
{

    #region Constructor
    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">Configurações de urls do sistema</param>
    /// <param name="host">Informações da aplicação em execução</param>
    public ControleMaterialEstoqueSaidaController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Controle de Material de Estoque e Saida
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">Lista de filtros selecionados para pesquisa de alunos</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.ControleMaterialEstoqueSaida, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);

        var response = ApiClientFactory.Instance.GetControlesMateriaisEstoquesSaidasAll();

        return View(new ControleMaterialEstoqueSaidaModel() { ControlesMateriaisEstoquesSaidas = response });
    }

    /// <summary>
    /// Tela para Inclusão de Controle de Material de Estoque e Saida
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.ControleMaterialEstoqueSaida, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var profissionais = new SelectList(ApiClientFactory.Instance.GetUsuarioAll().Where(x => x.Perfil.Id == (int)EnumPerfil.Profissional), "Id", "Nome");
            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");

            return View(new ControleMaterialEstoqueSaidaModel()
            {
                ListEstados = estados,
                ListProfissionais = profissionais
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Inclusão do ControleMaterialEstoqueSaida
    /// </summary>
    /// <param name="collection">Coleção de dados para inclusao de ControleMaterialEstoqueSaida</param>
    /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ControleMaterialEstoqueSaida, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var quantidade = Convert.ToInt32(collection["quantidade"].ToString());

            if (collection["e/s"].ToString() != "on")
            {
                quantidade = -quantidade;
            }

            var command0 = new ControleMaterialEstoqueSaidaModel.CreateUpdateControleMaterialEstoqueSaidaCommand
            {
                MunicipioId = Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                LocalidadeId = Convert.ToInt32(collection["ddlLocalidade"].ToString()),
                InventarioId = Convert.ToInt32(collection["ddlInventario"].ToString()),
                Quantidade = quantidade,
                ProfissionalId = Convert.ToInt32(collection["ddlProfissional"].ToString())
            };

            //var inventario = 
            //    ApiClientFactory.Instance.GetInventarioById(Convert.ToInt32(collection["ddlInventario"].ToString()));

            //var command1 = new InventarioModel.CreateUpdateInventarioCommand
            //{
            //    Id = inventario.Id,
            //    Quantidade = inventario.Quantidade + quantidade
            //};

            await ApiClientFactory.Instance.CreateControleMaterialEstoqueSaida(command0);
            //await ApiClientFactory.Instance.UpdateInventario(inventario.Id ,command1);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }


    /// <summary>
    /// Ação de Alteração Controle de Material de Estoque e Saida
    /// </summary>
    /// <param name="id">Identificador do ControleMaterialEstoqueSaida</param>
    /// <param name="collection">Coleção de dados para alteração de ControleMaterialEstoqueSaida</param>
    /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ControleMaterialEstoqueSaida, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new ControleMaterialEstoqueSaidaModel.CreateUpdateControleMaterialEstoqueSaidaCommand
            {
                Id = Convert.ToInt32(collection["editControleMaterialEstoqueSaidaId"]),
                ProfissionalId = Convert.ToInt32(collection["ddlProfissional"])
            };

            await ApiClientFactory.Instance.UpdateControleMaterialEstoqueSaida(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de exclusão de Controle de Material de Estoque e Saida
    /// </summary>
    /// <param name="id">Identificador Controle de Material de Estoque e Saida</param>
    /// <param name="collection">Coleção de dados para exclusão de ControleMaterialEstoqueSaida</param>
    /// <returns>Retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ControleMaterialEstoqueSaida, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            var controleSaida =
                ApiClientFactory.Instance.GetControleMaterialEstoqueSaidaById(id);

            //var inventario =
            //    ApiClientFactory.Instance.GetInventarioById(controleSaida.InventarioId);

            //var command = new InventarioModel.CreateUpdateInventarioCommand
            //{
            //    Id = inventario.Id,
            //    Quantidade = inventario.Quantidade - controleSaida.Quantidade
            //};

            //ApiClientFactory.Instance.UpdateInventario(inventario.Id, command);
            ApiClientFactory.Instance.DeleteControleMaterialEstoqueSaida(id);
            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Este grupo de inventario não pode ser excluído pois possui aulas vinculadas a ele." });
        }
    }

    //public Task<JsonResult> GetControlesMateriaisEstoquesSaidasByMaterialId(string id)
    //{
    //    try
    //    {
    //        if (string.IsNullOrEmpty(id)) throw new Exception("Material não informado.");
    //        var resultLocal = ApiClientFactory.Instance.GetControlesMateriaisEstoquesSaidasByMaterialId(Convert.ToInt32(id));

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
    /// Busca de Controle de Material de Estoque e Saida por Id
    /// </summary>
    /// <param name="id">Identificador de Controle de Material de Estoque e Saida </param>
    /// <returns>Retorna o Controle de Material de Estoque e Saida</returns>
    public Task<ControleMaterialEstoqueSaidaDto> GetControleMaterialEstoqueSaidaById(int id)
    {
        var result = ApiClientFactory.Instance.GetControleMaterialEstoqueSaidaById(id);
        var profissionais = new SelectList(ApiClientFactory.Instance.GetUsuarioAll().Where(x => x.Perfil.Id == (int)EnumPerfil.Profissional), "Id", "Nome", result.ProfissionalId);

        result.ListProfissionais = profissionais;

        return Task.FromResult(result);
    }

    /// <summary>
    /// Método de busca todos os Municipios pelo nome do Estado
    /// </summary>
    /// <param name="id">Sigla do Estado</param>
    /// <returns>Retorna um json com todos os municipios</returns>
    public Task<JsonResult> GetMunicipiosByUf(string uf)
    {
        try
        {
            if (string.IsNullOrEmpty(uf)) throw new Exception("Uf não informada.");
            var resultLocal = ApiClientFactory.Instance.GetMunicipiosByUf(uf);

            return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Nome")));

        }
        catch (Exception ex)
        {
            return Task.FromResult(Json(ex.Message));
        }
    }

    /// <summary>
    /// Método de busca todos as Localidades pelo nome do municipio
    /// </summary>
    /// <param name="id">Nome do Municipio</param>
    /// <returns>Retorna um json com todos as localidades</returns>
    public Task<JsonResult> GetLocalidadeByMunicipio(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("Municipio não informado.");
            var resultLocal = ApiClientFactory.Instance.GetLocalidadeByMunicipioId(id);

            return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Nome")));

        }
        catch (Exception ex)
        {
            return Task.FromResult(Json(ex.Message));
        }
    }
    #endregion
}