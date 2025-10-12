using log4net;
using Microsoft.AspNetCore.Authorization;
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

namespace WebApp.Controllers
{
    /// <summary>
    /// Controle de Disciplina 
    /// </summary>
    [Authorize(Policy = ModuloAccess.ConfiguracaoSistema)]
    public class DisciplinaController : BaseController
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
        public DisciplinaController(IOptions<UrlSettings> appSettings, ILog logger)
        {
            _logger = logger;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods
        /// <summary>
        /// Listagem de Disciplina
        /// </summary>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        [ClaimsAuthorize(ClaimType.Disciplina, Identity.Claim.Consultar)]
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            try
            {
                _logger.Info($"Usuario Logado em Disciplina.Index User.Identity.Name : {User.Identity.Name}");

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);
                var response = ApiClientFactory.Instance.GetDisciplinasAll();

                return View(new DisciplinaModel() { Disciplinas = response });
            }
            catch (Exception e)
            {
                _logger.Error($"Disciplina.Index: {e.StackTrace}");
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
        /// Listagem de Disciplina
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>Returns true false</returns>
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            return View();
        }

        /// <summary>
        ///  Ação de Inclusão de Disciplina 
        /// </summary>
        /// <param name="collection">Coleção de dados para inclusao de Disciplina</param>
        /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new DisciplinaModel.CreateUpdateDisciplinaCommand
                {
                    Nome = collection["nome"].ToString(),
                };

                await ApiClientFactory.Instance.CreateDisciplina(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Ação de Alteração de Disciplina 
        /// </summary>
        /// <param name="collection">Coleção de dados para alteração de Disciplina</param>
        /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            var command = new DisciplinaModel.CreateUpdateDisciplinaCommand
            {
                Id = Convert.ToInt32(collection["editDisciplinaId"]),
                Nome = collection["nome"].ToString(),
            };

            await ApiClientFactory.Instance.UpdateDisciplina(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        /// <summary>
        ///  Ação de Exclusão de Disciplina
        /// </summary>
        /// <param name="id">Identificador de Disciplina</param>
        /// <returns>Retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteDisciplina(id);
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
        /// Busca Disciplina por Id
        /// </summary>
        /// <param name="id">Identificador de Disciplina</param>
        /// <returns>Retorna a Categoria</returns>
        public Task<DisciplinaDto> GetDisciplinaById(int id)
        {
            var result = ApiClientFactory.Instance.GetDisciplinaById(id);

            return Task.FromResult(result);
        }

        #endregion

    }
}
