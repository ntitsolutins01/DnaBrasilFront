using log4net;
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
/// Controle Acesso Aula
/// </summary>
public class ControleAcessoAulaController : BaseController
{
    #region Parametros
    private readonly IOptions<UrlSettings> _appSettings;
    private readonly ILog _logger;
    #endregion

    #region Constructor
    /// <summary>
    /// Construtor da página
    /// </summary>
    /// <param name="appSettings">configurações de urls do sistema</param>
    /// <param name="host">informações da aplicação em execução</param>
    /// <param name="logger">Log de mensagens da aplicação</param>
    public ControleAcessoAulaController(IOptions<UrlSettings> appSettings,
        IWebHostEnvironment host,
        ILog logger)
    {
        ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
        _logger = logger;
    }
    #endregion

    #region Main Methods
    /// <summary>
    /// Listagem de Controle Acesso Aula
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="collection">Lista de filtros selecionados para pesquisa de alunos</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.ControleAcessoAula, Identity.Claim.Consultar)]
    public IActionResult Index(int? crud, int? notify, string message = null)
    {
        
        try
        {
            _logger.Info($"Usuario Logado em ControleAcessoAula.Index User.Identity.Name : {User.Identity.Name}");

            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetControlesAcessosAulasAll();

            return View(new ControleAcessoAulaModel() { ControlesAcessosAulas = response });

        }
        catch (Exception e)
        {
            _logger.Error($"ControleAcessoAula.Index: {e.StackTrace}");
            return RedirectToRoute(new
            {
                controller = "Home",
                action = "Error",
                message = e.Message,
                stackTrace = e.StackTrace
            });
        }
    }

    /// <summary>
    /// Tela para Inclusão de Controle Acesso Aula
    /// </summary>
    /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
    /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
    /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
    [ClaimsAuthorize(ClaimType.ControleAcessoAula, Identity.Claim.Incluir)]
    public ActionResult Create(int? crud, int? notify, string message = null)
    {
        try
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);



            return View(new ControleAcessoAulaModel()
            {
            });
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

        }
    }

    /// <summary>
    /// Ação de Inclusão do Controle Acesso Aula
    /// </summary>
    /// <param name="collection">Coleção de dados para inclusao de ControleAcessoAula</param>
    /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ControleAcessoAula, Identity.Claim.Incluir)]
    [HttpPost]
    public async Task<ActionResult> Create(IFormCollection collection)
    {
        try
        {
            var command = new ControleAcessoAulaModel.CreateUpdateControleAcessoAulaCommand
            {
                AulaId = 0,
                TempoPermanecia = null,
                LiberacaoAula = null,
                DataLiberacao = null,
                DataEncerramento = null
            };

            await ApiClientFactory.Instance.CreateControleAcessoAula(command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Tela Alteração de Controle Acesso Aula
    /// </summary>
    /// <param name="id">Id de alteração do ControleAcessoAula</param>
    /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    /// <exception cref="ArgumentNullException">Mensagem de erro ao alterar o ControleAcessoAula</exception>
    [ClaimsAuthorize(ClaimType.ControleAcessoAula, Claim.Alterar)]
    public ActionResult Edit(int id, int? crud, int? notify, string message = null)
    {
        try
        {

            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var ControleAcessoAula = ApiClientFactory.Instance.GetControleAcessoAulaById(id);


            return View(new ControleAcessoAulaModel()
            {


            });
        }
        catch (Exception ex)
        {
            return RedirectToAction(nameof(Index),
                new
                {
                    notify = (int)EnumNotify.Error,
                    message = $"Erro ao alterar usuário. Favor entrar em contato com o administrador do sistema. {ex.Message}"
                });
        }
    }

    /// <summary>
    /// Ação de Alteração do Controle Acesso Aula
    /// </summary>
    /// <param name="id">Identificador do ControleAcessoAula</param>
    /// <param name="collection">Coleção de dados para alteração de ControleAcessoAula</param>
    /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ControleAcessoAula, Identity.Claim.Alterar)]
    public async Task<ActionResult> Edit(IFormCollection collection)
    {
        try
        {
            var command = new ControleAcessoAulaModel.CreateUpdateControleAcessoAulaCommand
            {
                Id = Convert.ToInt32(collection["editControleAcessoAulaId"]),

                Status = collection["editStatus"]
                             .ToString() ==
                         ""
                    ? false
                    : true,
                AulaId = 0,
                TempoPermanecia = null,
                LiberacaoAula = null,
                DataLiberacao = null,
                DataEncerramento = null
            };

            await ApiClientFactory.Instance.UpdateControleAcessoAula(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
        }
    }

    /// <summary>
    /// Ação de Exclusão do Controle Acesso Aula
    /// </summary>
    /// <param name="id">Identificador do ControleAcessoAula</param>
    /// <param name="collection">Coleção de dados para exclusão de ControleAcessoAula</param>
    /// <returns>Retorna mensagem de exclusão através do parametro crud</returns>
    [ClaimsAuthorize(ClaimType.ControleAcessoAula, Identity.Claim.Excluir)]
    public ActionResult Delete(int id)
    {
        try
        {
            ApiClientFactory.Instance.DeleteControleAcessoAula(id);
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
    /// Busca Controle Acesso Aula por Id
    /// </summary>
    /// <param name="id">Identificador de Categoria</param>
    /// <returns>Retorna a Categoria</returns>
    public Task<ControleAcessoAulaDto> GetControleAcessoAulaById(int id)
    {
        var result = ApiClientFactory.Instance.GetControleAcessoAulaById(id);

        return Task.FromResult(result);
    }
    #endregion
}