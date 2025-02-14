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
    /// Controler Funcionalidade
    /// </summary>
    public class FuncionalidadeController : BaseController
    {
        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;


        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        public FuncionalidadeController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }


        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem de Funcionalidade
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>returns true false</returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetFuncionalidadesAll();

            return View(new FuncionalidadeModel() { Funcionalidades = response });
        }

        /// <summary>
        /// Tela para Inclusão de Funcionalidade
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>returns true false</returns>
        //[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            try
            {
                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);
                var modulos = new SelectList(ApiClientFactory.Instance.GetModulosAll(), "Id", "Nome");

                return View(new ModuloModel() { ListModulos = modulos });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        /// <summary>
        /// Ação de Inclusão de Funcionalidade
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao de Funcionalidade</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new FuncionalidadeModel.CreateUpdateFuncionalidadeCommand
                {
                    ModuloId = Convert.ToInt32(collection["ddlModulo"].ToString()),
                    Nome = collection["nome"].ToString()
                };

                await ApiClientFactory.Instance.CreateFuncionalidade(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = $"Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema. {e.Message}" });
            }
        }

        /// <summary>
        /// Ação de Alteração de Funcionalidade
        /// </summary>
        /// <param name="collection">coleção de dados para alteração de Funcionalidade</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            try
            {
                var command = new FuncionalidadeModel.CreateUpdateFuncionalidadeCommand
                {
                    Id = Convert.ToInt32(collection["editFuncionalidadeId"]),
                    Nome = collection["nome"].ToString()
                };

                await ApiClientFactory.Instance.UpdateFuncionalidade(command.Id, command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = $"Erro ao executar esta ação. Favor entrar em contato com o administrador do sistema. {e.Message}" });
            }
        }

        /// <summary>
        /// Ação de exclusão do Funcionalidade
        /// </summary>
        /// <param name="id">identificador do Funcionalidade</param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteFuncionalidade(id);
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
        /// Busca Funcionalidade por Id
        /// </summary>
        /// <param name="id">Identificador de Funcionalidade</param>
        /// <returns>Retorna a Funcionalidade</returns>
        public Task<FuncionalidadeDto> GetFuncionalidadeById(int id)
        {
            var result = ApiClientFactory.Instance.GetFuncionalidadeById(id);

            return Task.FromResult(result);
        }
    }

    #endregion


}
