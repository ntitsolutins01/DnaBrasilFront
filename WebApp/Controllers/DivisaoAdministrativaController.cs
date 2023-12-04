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
    public class DivisaoAdministrativaController : BaseController
	{
		private readonly ILogger<DivisaoAdministrativaController> _logger;
		private readonly IOptions<UrlSettings> _appSettings;


		public DivisaoAdministrativaController(ILogger<DivisaoAdministrativaController> logger, IOptions<UrlSettings> appSettings)
		{
			_logger = logger;
			_appSettings = appSettings;
			ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
		}
		
        public async Task<JsonResult> GetMunicipioByUf(string uf)
        {
            try
            {
                if (string.IsNullOrEmpty(uf)) throw new Exception("Estado não informado.");
                var resultLocal = ApiClientFactory.Instance.GetMunicipiosByUf(uf);

                return Json(new SelectList(resultLocal, "Id", "Nome"));

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
