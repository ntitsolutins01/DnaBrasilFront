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
/// Controle de Material
/// </summary>
[Authorize(Policy = ModuloAccess.ConfiguracaoSistemaEad)]
public class MaterialController : BaseController
{
    #region Constructor
    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="app">configurações de urls do sistema</param>
    /// <param name="host">informações da aplicação em execução</param>
    public MaterialController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Material
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">lista de filtros selecionados para pesquisa de materiais</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Material, Identity.Claim.Consultar)]
    public async Task<ActionResult> Index(int? crud, int? notify, IFormCollection collection, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);

        var searchFilter = new MateriaisFilterDto
        {
            Id = collection["material"].ToString(),
            NomeMaterial = collection["nomeMaterial"].ToString(),
            TipoMaterialId = collection["ddlTipoMaterial"].ToString(),
        };
        var result = await ApiClientFactory.Instance.GetMateriaisByFilter(searchFilter);

        var tiposMateriais = new SelectList(ApiClientFactory.Instance.GetTiposMateriaisAll(), "Id", "Nome");

        List<SelectListDto> list = new List<SelectListDto>
        {
            new() { IdNome = "CAIXA", Nome = "CAIXA" },
            new() { IdNome = "DIARIA", Nome = "DIARIA" },
            new() { IdNome = "KIT", Nome = "KIT" },
            new() { IdNome = "PACOTE", Nome = "PACOTE" },
            new() { IdNome = "SERVICOS", Nome = "SERVICOS" },
            new() { IdNome = "UNIDADE", Nome = "UNIDADE" },
            new() { IdNome = "UNIDADE KIT", Nome = "UNIDADE KIT" },
            new() { IdNome = "UNIDADE-PAR", Nome = "UNIDADE-PAR" }
        };

        var undMedidas = new SelectList(list, "IdNome", "Nome");

        var model = new MaterialModel
        {
            ListTiposMateriais = tiposMateriais,
            ListUnidadesMedidas = undMedidas,
            Materiais = result.Materiais,
            SearchFilter = searchFilter

        };
        return View(model);
    }

    /// <summary>
    /// Tela para Inclusão de Material
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.Material, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var tipoMateriais = new SelectList(ApiClientFactory.Instance.GetTiposMateriaisAll(), "Id", "Nome");

            List<SelectListDto> list = new List<SelectListDto>
            {
                new() { IdNome = "CAIXA", Nome = "CAIXA" },
                new() { IdNome = "DIARIA", Nome = "DIARIA" },
                new() { IdNome = "KIT", Nome = "KIT" },
                new() { IdNome = "PACOTE", Nome = "PACOTE" },
                new() { IdNome = "SERVICOS", Nome = "SERVICOS" },
                new() { IdNome = "UNIDADE", Nome = "UNIDADE" },
                new() { IdNome = "UNIDADE KIT", Nome = "UNIDADE KIT" },
                new() { IdNome = "UNIDADE-PAR", Nome = "UNIDADE-PAR" }
            };

            var undMedidas = new SelectList(list, "IdNome", "Nome");

            return View(new MaterialModel()
            {
                ListTiposMateriais = tipoMateriais,
                ListUnidadesMedidas = undMedidas
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Inclusão do Material
    /// </summary>
    /// <param name="collection">coleção de dados para Inclusao de Material</param>
    /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Material, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new MaterialModel.CreateUpdateMaterialCommand
            {
                TipoMaterialId = Convert.ToInt32(collection["ddlTipoMaterial"].ToString()),
                UnidadeMedida = collection["ddlUnidadeMedida"].ToString(),
                QtdAdquirida = Convert.ToInt32(collection["qtdAdquirida"]),
                Descricao = collection["descricao"].ToString()
            };

            await ApiClientFactory.Instance.CreateMaterial(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }


    /// <summary>
    /// Ação de Alteração do Material
    /// </summary>
    /// <param name="id">Identificador do Material</param>
    /// <param name="collection">coleção de dados para Alteração de Material</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Material, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var material =
                ApiClientFactory.Instance.GetMaterialById(Convert.ToInt32(collection["editMaterialId"]));

            var command = new MaterialModel.CreateUpdateMaterialCommand
            {
                Id = Convert.ToInt32(collection["editMaterialId"]),
                UnidadeMedida = collection["ddlUnidadeMedida"].ToString(),
                Descricao = collection["descricao"].ToString(),
                QtdAdquirida = material.QtdAdquirida
            };

            await ApiClientFactory.Instance.UpdateMaterial(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão do Material
    /// </summary>
    /// <param name="id">Identificador do Material</param>
    /// <param name="collection">coleção de dados para exclusão de Material</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.Material, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteMaterial(id);
            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Este grupo de material não pode ser excluído pois possui aulas vinculadas a ele." });
        }
    }

    public Task<JsonResult> GetMateriaisByTipoMaterialId(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id)) throw new Exception("Material não informado.");
            var resultLocal = ApiClientFactory.Instance.GetMateriaisByTipoMaterialId(Convert.ToInt32(id));

            return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Descricao")));

        }
        catch (Exception ex)
        {
            return Task.FromResult(Json(ex.Message));
        }
    }
    #endregion

    #region Get Methods

    /// <summary>
    /// Busca Materail por Id
    /// </summary>
    /// <param name="id">Identificador de Materail</param>
    /// <returns>Retorna a Material</returns>
    public Task<MaterialDto> GetMaterialById(int id)
    {
        var result = ApiClientFactory.Instance.GetMaterialById(id);

        return Task.FromResult(result);
    }
    #endregion
}