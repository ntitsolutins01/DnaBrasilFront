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
/// Controle de Grupo Material
/// </summary>
public class GrupoMaterialController : BaseController
{
    #region Constructor
    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="app">configurações de urls do sistema</param>
    /// <param name="host">informações da aplicação em execução</param>
    public GrupoMaterialController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Grupo Material
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">lista de filtros selecionados para pesquisa de alunos</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.GrupoMaterial, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetGruposMateriaisAll();

        return View(new GrupoMaterialModel() { GruposMateriais = response });
    }

    /// <summary>
    /// Tela para Inclusão de Grupo Material
    /// </summary>
    /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.GrupoMaterial, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            
            return View(new GrupoMaterialModel());
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Inclusão do Grupo Material
    /// </summary>
    /// <param name="collection">coleção de dados para inclusao de Grupo Material</param>
    /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.GrupoMaterial, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new GrupoMaterialModel.CreateUpdateGrupoMaterialCommand
            {
                Id = Convert.ToInt32(collection["editGrupoMaterialId"]),
                Nome = collection["Nome"].ToString()
            };

            //var possuiGrupoMaterial = ApiClientFactory.Instance.GetGrupoMaterialByAlunoIdDisciplinaId(Convert.ToInt32(command.AlunoId), Convert.ToInt32(command.DisciplinaId));

            //if (possuiGrupoMaterial==null)
            //{
            // return RedirectToAction(nameof(Create), new { notify = (int)EnumNotify.Warning, message = "Já existe GrupoMaterial cadastrada para este aluno na disciplina informada." });
            //}

            await ApiClientFactory.Instance.CreateGrupoMaterial(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Alteração do Grupo Material
    /// </summary>
    /// <param name="id">identificador do Grupo Material</param>
    /// <param name="collection">coleção de dados para alteração de Grupo Material</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.GrupoMaterial, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new GrupoMaterialModel.CreateUpdateGrupoMaterialCommand
            {
                Id = Convert.ToInt32(collection["editGrupoMaterialId"]),
                Nome = collection["Nome"].ToString()
            };

            await ApiClientFactory.Instance.UpdateGrupoMaterial(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão do Grupo Material
    /// </summary>
    /// <param name="id">identificador do Grupo Material</param>
    /// <param name="collection">coleção de dados para exclusão de Grupo Material</param>
    /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.GrupoMaterial, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteGrupoMaterial(id);
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
    ///  Busca Grupo Materail por id
    /// </summary>
    /// <param name="id">id do Grupo Material </param>
    /// <returns>retorna o objeto controle de Grupo Material</returns>
    [ClaimsAuthorize(ClaimType.GrupoMaterial, Identity.Claim.Consultar)]
    public Task<GrupoMaterialDto> GetGrupoMaterialById(int id)
    {
        var result = ApiClientFactory.Instance.GetGrupoMaterialById(id);

        return Task.FromResult(result);
    }
    #endregion
}