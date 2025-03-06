using DocumentFormat.OpenXml.Spreadsheet;
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
	/// Controle de Contrato
	/// </summary>
    public class ContratoController : BaseController
	{
        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">Configurações de urls do sistema</param>
        public ContratoController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }
        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem de Contrato
        /// </summary>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">Parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">Mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>Returns true false</returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetContratoAll();

            return View(new ContratoModel() { Contratos = response });
        }

        /// <summary>
        /// Tela para Inclusão de Contrato
        /// </summary>
        /// <param name="crud">Paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns>Returns true false</returns>
        //[ClaimsAuthorize("ConfiguracaoSistema", "Incluir")]
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            return View();
        }

        /// <summary>
        /// Ação de Inclusão de Contrato
        /// </summary>
        /// <param name="collection">Coleção de dados para inclusao de Contrato</param>
        /// <returns>Retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new ContratoModel.CreateUpdateContratoCommand
                {
                    Nome = collection["nome"].ToString().ToUpper(),
                    Descricao = collection["descricao"].ToString(),
                    DtIni = collection["dtini"].ToString(),
                    DtFim = collection["dtfim"].ToString(),
                    Anexo = collection["anexo"].ToString()
                };

                await ApiClientFactory.Instance.CreateContrato(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });

            }
        }

        /// <summary>
        ///  Ação de Alteração de Contrato
        /// </summary>
        /// <param name="collection">Coleção de dados para alteração de Categoria</param>
        /// <returns>Retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            var command = new ContratoModel.CreateUpdateContratoCommand
            {
                Id = Convert.ToInt32(collection["editContratoId"]),
                Nome = collection["nome"].ToString().ToUpper(),
                Descricao = collection["descricao"].ToString(),
                DtIni = collection["dtini"].ToString(),
                DtFim = collection["dtfim"].ToString(),
                Status = collection["editStatus"].ToString() == "" ? false : true,
                Anexo = collection["anexo"].ToString()
            };

            await ApiClientFactory.Instance.UpdateContrato(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        /// <summary>
        /// Ação de Exclusão de Contrato
        /// </summary>
        /// <param name="id">Identificador do Contrato</param>
        /// <returns>Retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteContrato(id);
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
        ///  Busca Contrato por Id
        /// </summary>
        /// <param name="id">Identificador de Contrato</param>
        /// <returns>Retorna a Contrato</returns>
        public Task<ContratoDto> GetContratoById(int id)
        {
            var result = ApiClientFactory.Instance.GetContratoById(id);

            return Task.FromResult(result);
        }

        #endregion










    }
}

