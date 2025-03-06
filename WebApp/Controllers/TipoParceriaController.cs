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
    /// Controle do Tipo Parceria 
    /// </summary>
    public class TipoParceriaController : BaseController
    {
        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        public TipoParceriaController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem do Tipo Parceria 
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetTipoParceriaAll();

            return View(new TipoParceriaModel() { TipoParcerias = response });
        }

        /// <summary>
        /// Tela para Inclusão do Tipo Parceria 
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
        /// Ação de Inclusão do Tipo Parceria 
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao do Tipo Parceria </param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new TipoParceriaModel.CreateUpdateTipoParceriaCommand
                {
                    Nome = collection["tipoParceria"].ToString(),
                    Parceria = Convert.ToInt32(collection["ddlParceria"].ToString()),
                    Status = true
                };

                await ApiClientFactory.Instance.CreateTipoParceria(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        ///  Ação de Alteração do Tipo Parceria 
        /// </summary>
        /// <param name="collection">coleção de dados para alteração do Tipo Parceria </param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            var command = new TipoParceriaModel.CreateUpdateTipoParceriaCommand
            {
                Id = Convert.ToInt32(collection["editTipoParceriaId"]),
                Nome = collection["nome"].ToString(),
                //Parceria = Convert.ToInt32(collection["ddlParceria"].ToString()),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            await ApiClientFactory.Instance.UpdateTipoParceria(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        /// <summary>
        /// Ação de Exclusão do Tipo Parceria 
        /// </summary>
        /// <param name="id">identificador do Tipo Parceria </param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteTipoParceria(id);
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
        /// Busca Tipo Parceria  por Id
        /// </summary>
        /// <param name="id">Identificador do Tipo Parceria</param>
        /// <returns>Retorna a Tipo Parceria</returns>
        public Task<TipoParceriaDto> GetTipoParceriaById(int id)
        {
            var result = ApiClientFactory.Instance.GetTipoParceriaById(id);

            return Task.FromResult(result);
        }
    }

    #endregion




}
