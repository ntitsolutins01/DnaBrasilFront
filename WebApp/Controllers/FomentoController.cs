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
    /// Controle de Fomento
    /// </summary>
    public class FomentoController : BaseController
    {

        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        /// <summary>
        /// Construtor da página
        /// </summary>
        /// <param name="appSettings">configurações de urls do sistema</param>
        public FomentoController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        ///  Listagem de Fomento
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {

            ViewBag.Status = true;
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetFomentoAll();
            var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeAll(), "Id", "Nome");
            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");

            return View(new FomentoModel()
            {
                Fomentos = response,
                ListLocalidades = localidades,
                ListEstados = estados
            });
        }

        /// <summary>
        /// Tela para Inclusão de Fomento
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public ActionResult Create(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
            var linhasAcoes = new SelectList(ApiClientFactory.Instance.GetLinhasAcoesAll(), "Id", "Nome");

            var model = new FomentoModel
            {
                ListEstados = estados,
                ListLinhasAcoes = linhasAcoes

            };
            return View(model);
        }

        /// <summary>
        /// Ação de Inclusão de Fomento
        /// </summary>
        /// <param name="collection">coleção de dados para Inclusao de Fomento</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new FomentoModel.CreateUpdateFomentoCommand
                {
                    LocalidadesIds = collection["ddlLocalidade"].ToString(),
                    LinhasAcoesIds = collection["ddlLinhaAcao"].ToString(),
                    LocalidadeId = 53,
                    MunicipioId = Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                    Codigo = collection["codigo"].ToString(),
                    DtIni = collection["dtIni"].ToString(),
                    DtFim = collection["dtFim"].ToString(),
                    Nome = collection["Nome"].ToString()
                };

                await ApiClientFactory.Instance.CreateFomento(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Ação de Alteração de Fomento
        /// </summary>
        /// <param name="id">Identificador de Fomento</param>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public ActionResult Edit(int id, int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);

            var fomento = ApiClientFactory.Instance.GetFomentoById(id);

            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", fomento.Sigla);
            var municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(fomento.Sigla), "Id", "Nome", fomento.MunicipioId);
            var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeByMunicipio(fomento.MunicipioId.ToString()), "Id", "Nome", fomento.LocalidadeId);
            var linhasAcoes = new SelectList(ApiClientFactory.Instance.GetLinhasAcoesAll(), "Id", "Nome", fomento.LinhasAcoesIds);

            var model = new FomentoModel
            {
                ListEstados = estados,
                ListLinhasAcoes = linhasAcoes,
                ListMunicipios = municipios,
                ListLocalidades = localidades,
                Fomento = fomento

            };
            return View(model);
        }

        /// <summary>
        /// Ação de Alteração de Fomento
        /// </summary>
        /// <param name="id">Identificador de Fomento</param>
        /// <param name="collection">coleção de dados para Alteração de Fomento</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        [HttpPost]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            var command = new FomentoModel.CreateUpdateFomentoCommand
            {
                Id = id,
                Codigo = collection["codigo"].ToString(),
                Nome = collection["Nome"].ToString(),
                DtIni = collection["dtIni"].ToString(),
                DtFim = collection["dtFim"].ToString(),
                LinhasAcoesIds = collection["ddlLinhaAcao"].ToString(),
                LocalidadesIds = collection["ddlLocalidade"].ToString(),
                LocalidadeId = 53,
                MunicipioId = Convert.ToInt32(collection["ddlMunicipio"].ToString()),
                Status = collection["status"].ToString() == "" ? false : true
            };

            await ApiClientFactory.Instance.UpdateFomento(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        /// <summary>
        /// Ação de Exclusão de Fomento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteFomento(id);
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
        /// Busca Fomento por Id
        /// </summary>
        /// <param name="id">Identificador de Fomento</param>
        /// <returns>Retorna a Fomento</returns>
        public Task<FomentoDto> GetFomentoById(int id)
        {
            var result = ApiClientFactory.Instance.GetFomentoById(id);

            return Task.FromResult(result);
        }

        /// <summary>
        /// Busca municipio por id de Fomento
        /// </summary>
        /// <param name="id">Identificador de Fomento</param>
        /// <returns>Retorna a um Fomento</returns>
        public Task<JsonResult> GetMunicipioIdByFomento(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Fomento não informado.");
                var fomento = ApiClientFactory.Instance.GetFomentoById(Convert.ToInt32(id));

                return Task.FromResult(Json(fomento.MunicipioId));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }
    }

    #endregion
}
