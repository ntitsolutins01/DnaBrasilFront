using log4net;
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
	/// Controle de Serie
	/// </summary>
	public class SerieController : BaseController
	{

        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;
        private readonly ILog _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        public SerieController(IOptions<UrlSettings> appSettings,
            ILog logger)
        {
            _appSettings = appSettings;
            _logger = logger;
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
            try
            {
                _logger.Info($"SerieController - Create");

                SetNotifyMessage(notify, message);
                SetCrudMessage(crud);

                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
                var etapas = new SelectList(ApiClientFactory.Instance.GetEtapasEnsinoAll(), "Id", "Nome");

                var model = new SerieModel()
                {
                    ListEstados = estados,
                    ListEtapas = etapas
                };

                return View(model);
            }
            catch (Exception e)
            {
                _logger.Error(e.StackTrace);
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = e.Message });
            }
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
                    Turma = collection["turma"].ToString(),
                    EtapaEnsinoId = Convert.ToInt32(collection["ddlEtapa"].ToString()),
                    LocalidadeId = Convert.ToInt32(collection["ddlLocalidade"].ToString())
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
                Turma = collection["turma"].ToString(),
                Status = collection["editStatus"].ToString() == "" ? false : true
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

        /// <summary>
        /// Busca Etapas por Localidade
        /// </summary>
        /// <param name="id">Identificador da Localidade</param>
        /// <returns>Retorna um json com a lista de etapas da Localidade</returns>
        public Task<JsonResult> GetEtapasByLocalidadeId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Localidade não informada.");
                var resultLocal = ApiClientFactory.Instance.GetEtapasByLocalidadeId(Convert.ToInt32(id));

                return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Nome")));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }

        /// <summary>
        /// Busca séries por localidade e etapa
        /// </summary>
        /// <param name="localidadeId">Id da localidade</param>
        /// <param name="etapaId">Id da etapa de ensino</param>
        /// <returns>Retorna um json com a lista de séries</returns>
        public Task<JsonResult> GetSeriesByLocalidadeIdEtapaId(string localidadeId, string etapaId)
        {
            try
            {
                if (string.IsNullOrEmpty(localidadeId)) throw new Exception("Localidade não informada.");
                if (string.IsNullOrEmpty(etapaId)) throw new Exception("Etapa de ensino não informada.");
                var result = ApiClientFactory.Instance
                    .GetSeriesByLocalidadeIdEtapaId(Convert.ToInt32(localidadeId), Convert.ToInt32(etapaId))
                    .Select(s => new { Id = s.Nome, Nome = s.Nome }).Distinct().ToList();

                return Task.FromResult(Json(new SelectList(result, "Nome", "Nome")));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }

        /// <summary>
        /// Busca turmas por série
        /// </summary>
        /// <param name="localidadeId">Id da localidade</param>
        /// <param name="etapaId">Id da etapa de ensino</param>
        /// <param name="serie">Série selecionada</param>
        /// <returns>Retorna um json com a lista de turmas</returns>
        public Task<JsonResult> GetTurmasByLocalidadeIdEtapaIdSerie(string localidadeId, string etapaId, string serie)
        {
            try
            {
                if (string.IsNullOrEmpty(localidadeId)) throw new Exception("Localidade não informada.");
                if (string.IsNullOrEmpty(etapaId)) throw new Exception("Etapa de ensino não informada.");
                if (string.IsNullOrEmpty(serie)) throw new Exception("Série não informada.");
                var result = ApiClientFactory.Instance
                    .GetTurmasByLocalidadeIdEtapaIdSerie(Convert.ToInt32(localidadeId), Convert.ToInt32(etapaId), serie)
                    .Select(s => new { Id = s.Id, Turma = s.Turma }).ToList();

                return Task.FromResult(Json(new SelectList(result, "Id", "Turma")));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }
        #endregion
    }
}
