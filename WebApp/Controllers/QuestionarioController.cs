using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
	/// <summary>
	/// Controle de Questionario
	/// </summary>
	public class QuestionarioController : BaseController
	{

        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        public QuestionarioController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem de Questionario
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetQuestionarioAll();
            var model = new QuestionarioModel()
            {
                Questionarios = response
            };

            return View(model);
        }

        /// <summary>
        /// Tela para Inclusão de Questionario
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        //[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var tiposlaudos = new SelectList(ApiClientFactory.Instance.GetTiposLaudoAll(), "Id", "Nome");
            var model = new QuestionarioModel()
            {
                ListTiposLaudos = tiposlaudos
            };
            return View(model);
        }

        /// <summary>
        /// Ação de Inclusão de Questionario
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao de Questionario</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new QuestionarioModel.CreateUpdateQuestionarioCommand
                {
                    Pergunta = collection["pergunta"].ToString(),
                    TipoLaudoId = Convert.ToInt32(collection["ddlTipoLaudo"].ToString()),
                    Quadrante = Convert.ToInt32(collection["quadrante"].ToString()),
                    Questao = Convert.ToInt32(collection["questao"].ToString()),

                };

                await ApiClientFactory.Instance.CreateQuestionario(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        /// <summary>
        /// Ação de Alteração de Questionario
        /// </summary>
        /// <param name="collection">Identificador de Questionario</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            try
            {
                var command = new QuestionarioModel.CreateUpdateQuestionarioCommand
                {
                    Id = Convert.ToInt32(collection["editQuestionarioId"]),
                    Pergunta = collection["pergunta"].ToString(),
                    Quadrante = Convert.ToInt32(collection["quadrante"].ToString()),
                    Questao = Convert.ToInt32(collection["questao"].ToString()),
                };

                await ApiClientFactory.Instance.UpdateQuestionario(command.Id, command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        /// <summary>
        /// Ação de Exclusão do Questionario
        /// </summary>
        /// <param name="id">identificador do Questionario</param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteQuestionario(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Busca Questionario pelo Id
        /// </summary>
        /// <param name="id">Identificador de Questionario</param>
        /// <returns>Retorna a Questionario</returns>
        public Task<QuestionarioDto> GetQuestionarioById(int id)
        {
            var result = ApiClientFactory.Instance.GetQuestionarioById(id);

            return Task.FromResult(result);
        }

        /// <summary>
        /// Busca Questionario por Tipo Laudo
        /// </summary>
        /// <param name="id">Identificador de Tipo Laudo</param>
        /// <returns>Retorna a Questionario</returns>
        public Task<JsonResult> GetQuestionariosByTipoLaudo(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Tipo de Laudo não informado.");
                var resultLocal = ApiClientFactory.Instance.GetQuestionarioByTipoLaudo(Convert.ToInt32(id));

                return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Pergunta")));

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return Task.FromResult(Json(e.Message));
            }
        }
    }

    #endregion

}
