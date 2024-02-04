﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Factory;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    public class DashboardController : BaseController
	{
		private readonly ILogger<DashboardController> _logger;
		private readonly IOptions<UrlSettings> _appSettings;


		public DashboardController(ILogger<DashboardController> logger, IOptions<UrlSettings> appSettings)
		{
			_logger = logger;
			_appSettings = appSettings;
			ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
		}

		public IActionResult Index()
		{
			var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentoAll(), "Id", "Nome");
			var localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeAll(), "Id", "Nome");
			var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome");
			var indicadores = ApiClientFactory.Instance.GetIndicadoresByFilter();

			var model = new DashboardModel
			{
				ListFomentos = fomentos,
				ListLocalidades = localidades,
				ListEstados = estados,
				Indicadores = indicadores

			};
			return View(model);
		}
		
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
                return Task.FromResult(Json(ex.Message));
            }
        }
    }
}
