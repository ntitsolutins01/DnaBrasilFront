using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controle de Divisao Administrativa
    /// </summary>
    public class DivisaoAdministrativaController : BaseController
	{
        #region Parametro

        private readonly ILogger<DivisaoAdministrativaController> _logger;
        private readonly IOptions<UrlSettings> _appSettings;

        #endregion

        #region Constructor

        /// <summary>
        ///  Construtor da página
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="appSettings">configurações de urls do sistema</param>
        public DivisaoAdministrativaController(ILogger<DivisaoAdministrativaController> logger, IOptions<UrlSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Get Methods

        /// <summary>
        ///  Busca Municipio por Uf
        /// </summary>
        /// <param name="uf">uf</param>
        /// <returns>Retorna a um Municipio</returns>
        public Task<JsonResult> GetMunicipioByUf(string uf)
        {
            try
            {
                if (string.IsNullOrEmpty(uf)) throw new Exception("Estado não informado.");
                var resultLocal = ApiClientFactory.Instance.GetMunicipiosByUf(uf);

                return Task.FromResult(Json(new SelectList(resultLocal, "Id", "Nome")));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }


        /// <summary>
        /// Busca um Municipio por Fomento
        /// </summary>
        /// <param name="id">Identificador de Municipio por Fomento</param>
        /// <returns>Retorna a um Municipio por fomento</returns>
        public Task<JsonResult> GetMunicipioByFomento(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) throw new Exception("Fomento não informado.");

                var fomento = ApiClientFactory.Instance.GetFomentoById(Convert.ToInt32(id));

                var result = new List<string>
                {
                    fomento.MunicipioId.ToString(),
                    fomento.MunicipioEstado.Split("/")[1].Trim()
                };

                return Task.FromResult(Json(result));

            }
            catch (Exception ex)
            {
                return Task.FromResult(Json(ex));
            }
        }
    }

    #endregion

}
