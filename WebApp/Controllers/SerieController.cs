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
	/// Controle de Serie
	/// </summary>
	public class SerieController : BaseController
	{

        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        public SerieController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        ///  Listagem de Serie
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetSerieAll();

            return View(new SerieModel() { Series = response });
        }

        /// <summary>
        /// Tela para Inclusão de Serie
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
        /// Ação de Inclusão de Serie
        /// </summary>
        /// <param name="collection">coleção de dados para inclusao de Serie</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new SerieModel.CreateUpdateSerieCommand
                {
                    Nome = collection["nome"].ToString(),
                    Descricao = collection["descricao"].ToString(),
                    IdadeInicial = Convert.ToInt32(collection["idadeIni"].ToString()),
                    IdadeFinal = Convert.ToInt32(collection["idadeFim"].ToString()),
                    ScoreTotal = Convert.ToInt32(collection["scoreTotal"].ToString())
                };

                await ApiClientFactory.Instance.CreateSerie(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Ação de Alteração de Serie
        /// </summary>
        /// <param name="collection">coleção de dados para alteração de Serie</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            var command = new SerieModel.CreateUpdateSerieCommand
            {
                Id = Convert.ToInt32(collection["editSerieId"]),
                Nome = collection["nome"].ToString(),
                Status = collection["editStatus"].ToString() == "" ? false : true,
                Descricao = collection["descricao"].ToString(),
                IdadeInicial = Convert.ToInt32(collection["idadeIni"].ToString()),
                IdadeFinal = Convert.ToInt32(collection["idadeFim"].ToString()),
                ScoreTotal = Convert.ToInt32(collection["scoreTotal"].ToString())
            };

            await ApiClientFactory.Instance.UpdateSerie(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        /// <summary>
        /// Ação de Exclusão de Serie
        /// </summary>
        /// <param name="id">identificador de Serie</param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteSerie(id);
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
        /// Busca Serie por Id
        /// </summary>
        /// <param name="id">Identificador de Serie</param>
        /// <returns>Retorna a Serie</returns>
        public Task<SerieDto> GetSerieById(int id)
        {
            var result = ApiClientFactory.Instance.GetSerieById(id);

            return Task.FromResult(result);
        }
    }

    #endregion



}
