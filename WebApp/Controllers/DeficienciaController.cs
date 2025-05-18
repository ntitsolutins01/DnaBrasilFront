using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers;

/// <summary>
/// Controle de Deficiencia
/// </summary>
public class DeficienciaController : BaseController
{
    #region Constructor

    private readonly IOptions<UrlSettings> _appSettings;

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">Configurações de urls do sistema</param>
    public DeficienciaController(IOptions<UrlSettings> appSettings)
    {
        _appSettings = appSettings;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }


    #endregion

    #region Main Methods

    /// <summary>
    /// Listagem de Deficiencia
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    /// <returns>Returns true false</returns>
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetDeficienciaAll();

        return View(new DeficienciaModel() { Deficiencias = response });
    }

    /// <summary>
    /// Tela para Inclusão de Deficiencia
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    /// <returns>Returns true false</returns>
    //[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);

        return View();
    }

    /// <summary>
    /// Ação de Inclusão de Deficiencia
    /// </summary>
    /// <param name="collection">Coleção de dados para inclusao de Deficiencia</param>
    /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
    //[ClaimsAuthorize("Usuario", "Incluir")]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new DeficienciaModel.CreateUpdateDeficienciaCommand
            {
                Nome = collection["nome"].ToString()
            };

            await ApiClientFactory.Instance.CreateDeficiencia(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Ação de Alteração de Deficiencia
    /// </summary>
    /// <param name="collection">Coleção de dados para alteração de Deficiencia</param>
    /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    //[ClaimsAuthorize("Usuario", "Alterar")]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        var command = new DeficienciaModel.CreateUpdateDeficienciaCommand
        {
            Id = Convert.ToInt32(collection["editDeficienciaId"]),
            Nome = collection["nome"].ToString(),
            Status = collection["editStatus"].ToString() == "" ? false : true
        };

        await ApiClientFactory.Instance.UpdateDeficiencia(command.Id, command);

        return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
    }

    /// <summary>
    /// Ação de Alteração de Deficiencia
    /// </summary>
    /// <param name="id">Identificador de Deficiencia</param>
    /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    //[ClaimsAuthorize("Usuario", "Excluir")]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteDeficiencia(id);
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
    /// Busca Deficiencia por Id
    /// </summary>
    /// <param name="id">Identificador de Deficiencia</param>
    /// <returns>Retorna a uma Deficiencia</returns>
    public Task<DeficienciaDto> GetDeficienciaById(int id)
    {
        var result = ApiClientFactory.Instance.GetDeficienciaById(id);

        return Task.FromResult(result);
    }

    #endregion

}