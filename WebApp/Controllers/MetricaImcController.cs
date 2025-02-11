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
	/// Controle de Metricalmc
	/// </summary>
	public class MetricaImcController : BaseController
	{

        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        public MetricaImcController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem de Metricalmc
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetMetricasImcAll();

            return View(new MetricaImcModel() { MetricasImc = response });
        }

        /// <summary>
        /// Tela para Inclusão de Metricalmc
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        //[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);
                var metricas = new SelectList(ApiClientFactory.Instance.GetMetricasImcAll(), "Id", "Nome");

                return View(new MetricaImcModel() { ListMetricasImc = metricas });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        /// <summary>
        /// Ação de Inclusão de Metricalmc
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao de Metricalmc</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new MetricaImcModel.CreateUpdateMetricaImcCommand
                {
                    Classificacao = collection["classificacao"].ToString(),
                    Idade = Convert.ToInt32(collection["idade"].ToString()),
                    ValorInicial = Convert.ToDecimal(collection["valorInicial"].ToString()),
                    ValorFinal = Convert.ToDecimal(collection["valorFinal"].ToString()),
                    Sexo = collection["ddlSexo"].ToString()
                };

                await ApiClientFactory.Instance.CreateMetricaImc(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
            }
        }

        /// <summary>
        /// Ação de Alteração de Metricalmc
        /// </summary>
        /// <param name="collection">coleção de dados para alteração de Metricalmc</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            try
            {
                var command = new MetricaImcModel.CreateUpdateMetricaImcCommand
                {
                    Id = Convert.ToInt32(collection["editMetricaImcId"]),
                    Classificacao = collection["classificacao"].ToString(),
                    Idade = Convert.ToInt32(collection["idade"].ToString()),
                    ValorInicial = Convert.ToDecimal(collection["valorInicial"].ToString()),
                    ValorFinal = Convert.ToDecimal(collection["valorFinal"].ToString()),
                    Status = collection["editStatus"].ToString() == "" ? false : true
                };

                await ApiClientFactory.Instance.UpdateMetricaImc(command.Id, command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = "Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema." });
            }
        }

        /// <summary>
        /// Ação de Exclusão do Metricalmc
        /// </summary>
        /// <param name="id">identificador do Metricalmc</param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteMetricaImc(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region  Get Methods

        /// <summary>
        /// Busca Metricalmc por Id
        /// </summary>
        /// <param name="id">Identificador de  Metricalmc</param>
        /// <returns>Retorna a Metricalmc</returns>
        public Task<MetricaImcDto> GetMetricaImcById(int id)
        {
            var result = ApiClientFactory.Instance.GetMetricaImcById(id);

            return Task.FromResult(result);
        }
    }

    #endregion


}
