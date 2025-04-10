using DocumentFormat.OpenXml.Office2010.Excel;
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
    /// Controle do Tipo de  Laudo 
    /// </summary>
    public class TiposLaudoController : BaseController
    {
        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        public TiposLaudoController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        ///  Listagem do Tipo de Laudo 
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetTiposLaudoAll();

            return View(new TiposLaudoModel() { TiposLaudos = response });
        }

        /// <summary>
        /// Tela para Inclusão do Tipo de Laudo 
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
        /// Ação de Inclusão do Tipo de Laudo 
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao do Tipo de  Laudo </param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new TiposLaudoModel.CreateUpdateTiposLaudoCommand
                {
                    Nome = collection["nome"].ToString(),
                    Descricao = collection["descricao"].ToString(),
                    IdadeMinima = Convert.ToInt32(collection["idade"])
                };

                await ApiClientFactory.Instance.CreateTiposLaudo(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Ação de Alteração do Tipo de Laudo 
        /// </summary>
        /// <param name="collection">coleção de dados para alteração do Tipo de  Laudo</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            var command = new TiposLaudoModel.CreateUpdateTiposLaudoCommand
            {
                Id = Convert.ToInt32(collection["editTiposLaudoId"]),
                Nome = collection["nome"].ToString(),
                Descricao = collection["descricao"].ToString(),
                IdadeMinima = Convert.ToInt32(collection["idade"]),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            await ApiClientFactory.Instance.UpdateTiposLaudo(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        /// <summary>
        /// Ação de Exclusão do Tipo de Laudo 
        /// </summary>
        /// <param name="id">identificador do Tipo de  Laudo </param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteTiposLaudo(id);
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
        /// Busca Tipo de Laudo  por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<TiposLaudoDto> GetTiposLaudoById(int id)
        {
            var result = ApiClientFactory.Instance.GetTiposLaudoById(id);

            return Task.FromResult(result);
        }
    }

    #endregion


}
