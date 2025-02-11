using Microsoft.AspNetCore.Mvc;
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
	/// Controle Modulo
	/// </summary>
	public class ModuloController : BaseController
	{
        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        public ModuloController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem de Modulo
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetModulosAll();

            return View(new ModuloModel() { Modulos = response });
        }

        /// <summary>
        /// Tela para Inclusão de Modulo
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

            return View();
        }

        /// <summary>
        /// Ação de Inclusão de Modulo
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao de Modulo</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new ConfiguracaoSistemaModel.CreateUpdateModuloCommand
                {
                    Nome = collection["nome"].ToString(),
                };

                await ApiClientFactory.Instance.CreateModulo(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Ação de Alteração de Modulo
        /// </summary>
        /// <param name="collection">coleção de dados para alteração de Modulo</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            var command = new ConfiguracaoSistemaModel.CreateUpdateModuloCommand
            {
                Id = Convert.ToInt32(collection["editModuloId"]),
                Nome = collection["nome"].ToString(),
            };

            await ApiClientFactory.Instance.UpdateModulo(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        /// <summary>
        /// Ação de Exclusão do Modulo
        /// </summary>
        /// <param name="id">identificador de Modulo</param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteModulo(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = $"ATENÇÃO. {e.Message}" });
            }
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Busca Modulo por Id
        /// </summary>
        /// <param name="id">Identificador de Modulo</param>
        /// <returns>Retorna a Modulo</returns>
        public Task<ModuloDto> GetModuloById(int id)
        {
            var result = ApiClientFactory.Instance.GetModuloById(id);

            return Task.FromResult(result);
        }
    }

    #endregion


}
