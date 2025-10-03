using log4net;
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
/// Controle de Etapa de Ensino
/// </summary>
public class EtapaEnsinoController : BaseController
{

    #region Parametros

    private readonly IOptions<UrlSettings> _appSettings;
    private readonly ILog _logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">Configurações de urls do sistema</param>
    public EtapaEnsinoController(IOptions<UrlSettings> appSettings,
        ILog logger)
    {
        _appSettings = appSettings;
        _logger = logger;
        ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
    }

    #endregion

    #region Main Methods

    /// <summary>
    /// Listagem de Eyapa de Ensino
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    /// <returns>Returns true false</returns>
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        SetNotifyMessage(notify, message);
        SetCrudMessage(crud);
        var response = ApiClientFactory.Instance.GetEtapasEnsinoAll();

        return View(new EtapaEnsinoModel() { EtapasEnsino = response });
    }

    /// <summary>
    /// Tela para Inclusão de Etapa de Ensino
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    /// <returns>Returns true false</returns>

    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            _logger.Info($"EtapaEnsinoController - Create");

            SetNotifyMessage(notify, message);

            var model = new EtapaEnsinoModel();

            return View(model);
        }
        catch (Exception e)
        {
            _logger.Error(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
        }
    }

    /// <summary>
    /// Ação de Inclusão de Etapa de Ensino
    /// </summary>
    /// <param name="collection">Coleção de dados para inclusao de Deficiencia</param>
    /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>

    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new EtapaEnsinoModel.CreateEtapaEnsinoCommand
            {
                Nome = collection["nome"].ToString(),

            };

            await ApiClientFactory.Instance.CreateEtapaEnsino(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Ação de Alteração de Etapa de Ensino
    /// </summary>
    /// <param name="collection">coleção de dados para alteração de Serie</param>
    /// <returns>retorna mensagem de alteração através do parametro crud</returns>

    public async Task<ActionResult> Edit(IFormCollection collection)
    {

        var command = new EtapaEnsinoModel.UpdateEtapaEnsinoCommand

        {
            Id = Convert.ToInt32(collection["id"]),
            Nome = collection["nome"].ToString(),
            Status = collection["status"].ToString() == "" ? false : true
        }
        ;

        await ApiClientFactory.Instance.UpdateEtapaEnsino(command.Id, command);

        return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
    }

    /// <summary>
    /// Ação de Alteração de Etapa de Ensino
    /// </summary>
    /// <param name="id">Identificador de Deficiencia</param>
    /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteEtapaEnsino(id);
            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
        }
    }

    #endregion

    #region Get Methods

    /// <summary>
    /// Busca Etapa de Ensino por Id
    /// </summary>
    /// <param name="id">Identificador de Deficiencia</param>
    /// <returns>Retorna a uma Deficiencia</returns>
    public Task<EtapaEnsinoDto> GetEtapaEnsinoById(int id)
    {
        var result = ApiClientFactory.Instance.GetEtapaEnsinoById(id);

        return Task.FromResult(result);
    }

    #endregion
}



