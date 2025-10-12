using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controle Linha de Acao
    /// </summary>
    [Authorize(Policy = ModuloAccess.ConfiguracaoSistema)]
    public class LinhaAcaoController : BaseController
    {

        #region Parametros

        private readonly ILog _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de url da api</param>
        /// <param name="logger">Log de mensagens da aplicação</param>
        public LinhaAcaoController(IOptions<UrlSettings> appSettings, ILog logger)
        {
            _logger = logger;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem de Linha de Acao 
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            
            try
            {
                _logger.Info($"Usuario Logado em LinhaAcao.Index User.Identity.Name : {User.Identity.Name}");

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);
                var response = ApiClientFactory.Instance.GetLinhasAcoesAll();

                return View(new LinhaAcaoModel() { LinhasAcoes = response });
            }
            catch (Exception e)
            {
                _logger.Error($"LinhaAcao.Index: {e.StackTrace}");
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
        /// Tela para Inclusão de Linha de Acao
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>returns a true false</returns>
        //[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);


            return View();
        }

        /// <summary>
        /// Ação de Inclusão de Linha de Acao
        /// </summary>
        /// <param name="collection">coleção de dados para Inclusao de Linha de Acao</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new LinhaAcaoModel.CreateUpdateLinhaAcaoCommand
                {
                    Nome = collection["tipoParceria"].ToString(),
                    Status = true
                };

                await ApiClientFactory.Instance.CreateLinhaAcao(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        ///  Ação de Alteração de Linha de Acao
        /// </summary>
        /// <param name="collection">coleção de dados para alteração de Linha de Acao</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            var command = new LinhaAcaoModel.CreateUpdateLinhaAcaoCommand
            {
                Id = Convert.ToInt32(collection["editLinhaAcaoId"]),
                Nome = collection["nome"].ToString(),
                //Parceria = Convert.ToInt32(collection["ddlParceria"].ToString()),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            await ApiClientFactory.Instance.UpdateLinhaAcao(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        /// <summary>
        /// Ação de Exclusão de Linha de Acao
        /// </summary>
        /// <param name="id">identificador do Categoria</param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteLinhaAcao(id);
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
        ///  Busca Linha de Acao por Id
        /// </summary>
        /// <param name="id">Identificador de Categoria</param>
        /// <returns>Retorna a Linha de Acao</returns>
        public Task<LinhaAcaoDto> GetLinhaAcaoById(int id)
        {
            var result = ApiClientFactory.Instance.GetLinhaAcaoById(id);

            return Task.FromResult(result);
        }
    }

    #endregion



}
