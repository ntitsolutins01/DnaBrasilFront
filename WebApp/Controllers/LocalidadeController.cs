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
    /// Controle de Localidade
    /// </summary>
    public class LocalidadeController : BaseController
    {

        #region Parametros

        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        public LocalidadeController(IOptions<UrlSettings> appSettings)
        {
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Main Methods

        /// <summary>
        /// Listagem de Localidade
        /// </summary>
        /// <param name="crud">paramentro que indica o tipo de ação realizado</param>
        /// <param name="notify">parametro que indica o tipo de notificação realizada</param>
        /// <param name="message">mensagem apresentada nas notificações e alertas gerados na tela</param>
        /// <returns></returns>
        public IActionResult Index(int? crud, int? notify, string message = null)
        {
            SetNotifyMessage(notify, message);
            SetCrudMessage(crud);
            var response = ApiClientFactory.Instance.GetLocalidadeAll();

            return View(new LocalidadeModel() { Localidades = response });
        }

        /// <summary>
        /// Tela para Inclusão da Localidade
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

            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");

            return View(new LocalidadeModel() { ListEstados = estados });
        }

        /// <summary>
        /// Ação de Inclusão da Localidade
        /// </summary>
        /// <param name="collection">coleção de dados para Inclusao de Localidade</param>
        /// <returns>retorna mensagem de inclusao através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Incluir")]
        [HttpPost]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var command = new LocalidadeModel.CreateUpdateLocalidadeCommand
                {
                    Nome = collection["nome"].ToString(),
                    Descricao = collection["descricao"].ToString(),
                    MunicipioId = Convert.ToInt32(collection["ddlMunicipio"].ToString())
                };

                await ApiClientFactory.Instance.CreateLocalidade(command);

                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Created });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Ação de Alteração da Localidade
        /// </summary>
        /// <param name="collection">coleção de dados para alteração de Localidade</param>
        /// <returns>retorna mensagem de alteração através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Alterar")]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            var command = new LocalidadeModel.CreateUpdateLocalidadeCommand
            {
                Id = Convert.ToInt32(collection["editLocalidadeId"]),
                Nome = collection["nome"].ToString(),
                Descricao = collection["descricao"].ToString(),
                MunicipioId = Convert.ToInt32(collection["editMunicipioId"].ToString()),
                Status = collection["editStatus"].ToString() == "" ? false : true
            };

            await ApiClientFactory.Instance.UpdateLocalidade(command.Id, command);

            return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Updated });
        }

        /// <summary>
        /// Ação de Exclusão da Localidade
        /// </summary>
        /// <param name="id">identificador da Localidade</param>
        /// <returns>retorna mensagem de exclusão através do parametro crud</returns>
        //[ClaimsAuthorize("Usuario", "Excluir")]
        public ActionResult Delete(int id)
        {
            try
            {
                ApiClientFactory.Instance.DeleteLocalidade(id);
                return RedirectToAction(nameof(Index), new { crud = (int)EnumCrud.Deleted });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index), new { notify = (int)EnumNotify.Error, message = $"Erro ao executar esta ação {e.Message}" });
            }
        }

        #endregion

        #region Get Methods

        /// <summary>
        ///  Busca Localidade por Id
        /// </summary>
        /// <param name="id">Identificador de Localidade</param>
        /// <returns>Retorna a Localidade</returns>
        public Task<LocalidadeDto> GetLocalidadeById(int id)
        {
            var result = ApiClientFactory.Instance.GetLocalidadeById(id);

            return Task.FromResult(result);
        }

        /// <summary>
        /// Busca Localidade por Municipio
        /// </summary>
        /// <param name="id">Identificador de Localidade por Municipio</param>
        /// <returns>Retorna a Localidade por Municipio</returns>
        public Task<JsonResult> GetLocalidadeByMunicipio(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Município não informado.");
                var resultLocal = ApiClientFactory.Instance.GetLocalidadeByMunicipio(id);

                return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Nome")));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }

        /// <summary>
        /// Busca Localidade por Fomento
        /// </summary>
        /// <param name="id">Identificador de Localidade por Fomento</param>
        /// <returns>Retorna a Localidade por Fomento</returns>
        public Task<JsonResult> GetLocalidadeByFomento(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Município não informado.");
                var resultLocal = ApiClientFactory.Instance.GetLocalidadeByFomento(Convert.ToInt32(id));

                return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Nome")));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }

        /// <summary>
        /// Busca Profissional por Localidade
        /// </summary>
        /// <param name="id">Identificador do Profissional por Localidade</param>
        /// <returns>Retorna o Profissional por Localidade</returns>
        public Task<JsonResult> GetProfissionaisByLocalidade(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Localidadee não informada.");
                var resultLocal = ApiClientFactory.Instance.GetProfissionaisByLocalidade(Convert.ToInt32(id));

                return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Nome")));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }
    }

    #endregion


}
