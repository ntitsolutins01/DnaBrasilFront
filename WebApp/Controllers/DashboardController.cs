using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    [Authorize(Policy = ModuloAccess.Dashboard)]
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
        public async Task<IActionResult> Index(IFormCollection collection)
        {
            //var usu = User?.Identity.Name;
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
            //email = User.FindFirstValue(ClaimTypes.Email);

            var searchFilter = new DashboardDto
            {
                FomentoId = collection["ddlFomento"].ToString(),
                Estado = collection["ddlEstado"].ToString(),
                MunicipioId = collection["ddlMunicipio"].ToString(),
                LocalidadeId = collection["ddlLocalidade"].ToString(),
                DeficienciaId = collection["ddlDeficiencia"].ToString(),
                Etnia = collection["ddlEtnia"].ToString()
            };

            var dashboard = new DashboardDto();

            var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentoAll(), "Id", "Nome", dashboard.FomentoId);
            var deficiencias = new SelectList(ApiClientFactory.Instance.GetDeficienciaAll().Where(x=>x.Status), "Id", "Nome", dashboard.DeficienciaId);
            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", dashboard.Estado);

            List<SelectListDto> list = new List<SelectListDto>
            {
                new() { IdNome = "PARDO", Nome = "PARDO" },
                new() { IdNome = "BRANCO", Nome = "BRANCO" },
                new() { IdNome = "PRETO", Nome = "PRETO" },
                new() { IdNome = "INDIGENA", Nome = "INDIGENA" },
                new() { IdNome = "AMARELO", Nome = "AMARELO" }
            };

            var etnias = new SelectList(list, "IdNome", "Nome", dashboard.Etnia);
            SelectList municipios = null;

            if (!string.IsNullOrEmpty(dashboard.Estado))
            {
                municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(dashboard.Estado), "Id", "Nome", dashboard.MunicipioId);
            }
            SelectList localidades = null;

            if (!string.IsNullOrEmpty(dashboard.LocalidadeId))
            {
                localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeByMunicipio(dashboard.MunicipioId), "Id", "Nome", dashboard.LocalidadeId);
            }

            var model = new DashboardModel
            {
                ListFomentos = fomentos,
                ListEstados = estados,
                Dashboard = dashboard,
                ListDeficiencias = deficiencias,
                ListMunicipios = municipios!,
                ListEtnias = etnias,
                ListLocalidades = localidades!
            };

            model.Dashboard.StatusLaudos = new StatusLaudosDto();


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
        public async Task<JsonResult> GetIndicadoresAlunosByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetIndicadoresAlunosByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return Json(model);

            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        public async Task<JsonResult> GetControlePresencaByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetControlePresencaByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetLaudosPeriodoByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetLaudosPeriodoByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetStatusLaudosByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetStatusLaudosByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetEvolutivoByFilter([FromBody] DashboardDto search)
        {
            try
            {
                //var dashboard = await ApiClientFactory.Instance.GetEvolutivoByFilter(search);

                var model = new DashboardModel
                {
                    //Dashboard = dashboard,
                    Dashboard = search,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetGraficosSaudeByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetGraficosSaudeByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetGraficosSaudeBucalByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetGraficosSaudeBucalByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetGraficosEtniaByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetGraficosEtniaByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetGraficosDeficienciasByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetGraficosDeficienciasByFilter(search);

                List<string> listPercDeficienciaCategorias = new List<string>();

                var listPercDeficiencia = new List<DataGrafico>();

                foreach (var item in dashboard.ListTotalizadorDeficiencia.PercDeficiencia)
                {
                    listPercDeficiencia.Add(new DataGrafico()
                    {
                        name = item.Key,
                        y = item.Value,
                        z = 50
                    });

                    listPercDeficienciaCategorias.Add(new string(item.Key));
                }

                var listValorDeficienciaMasc = new List<DataGrafico>();

                foreach (var item in dashboard.ListTotalizadorDeficiencia.ValorTotalizadorDeficienciaMasculino!)
                {
                    listValorDeficienciaMasc.Add(new DataGrafico()
                    {
                        name = item.Key,
                        y = item.Value,
                        z = 50
                    });

                }
                var listValorDeficienciaFem = new List<DataGrafico>();

                foreach (var item in dashboard.ListTotalizadorDeficiencia.ValorTotalizadorDeficienciaFeminino!)
                {
                    listValorDeficienciaFem.Add(new DataGrafico()
                    {
                        name = item.Key,
                        y = item.Value,
                        z = 50
                    });

                }

                dashboard.ListPercDeficiencia = listPercDeficiencia;
                dashboard.ListPercDeficienciaCategorias = listPercDeficienciaCategorias;
                dashboard.ListValorDeficienciaMasc = listValorDeficienciaMasc;
                dashboard.ListValorDeficienciaFem = listValorDeficienciaFem;

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetGraficosTalentoByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetGraficosTalentoByFilter(search);

                List<string> listPercTalentoCategorias = new List<string>();

                var listPercTalento = new List<DataGrafico>();

                foreach (var item in dashboard.ListTotalizadorTalento.PercTalento)
                {
                    listPercTalento.Add(new DataGrafico()
                    {
                        name = item.Key,
                        y = item.Value,
                        z = 50
                    });

                    listPercTalentoCategorias.Add(new string(item.Key));
                }

                var listValorTalentoMasc = new List<DataGrafico>();

                foreach (var item in dashboard.ListTotalizadorTalento.ValorTotalizadorTalentoMasculino!)
                {
                    listValorTalentoMasc.Add(new DataGrafico()
                    {
                        name = item.Key,
                        y = item.Value,
                        z = 50
                    });

                }
                var listValorTalentoFem = new List<DataGrafico>();

                foreach (var item in dashboard.ListTotalizadorTalento.ValorTotalizadorTalentoFeminino!)
                {
                    listValorTalentoFem.Add(new DataGrafico()
                    {
                        name = item.Key,
                        y = item.Value,
                        z = 50
                    });

                }

                dashboard.ListPercTalento = listPercTalento;
                dashboard.ListPercTalentoCategorias = listPercTalentoCategorias;
                dashboard.ListValorTalentoMasc = listValorTalentoMasc;
                dashboard.ListValorTalentoFem = listValorTalentoFem;

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetGraficoPercDesempenhoFisicoMotorByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetGraficoPercDesempenhoFisicoMotorByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetGraficosQualidadeVidaByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetGraficosQualidadeVidaByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetGraficosConsumoAlimentarByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetGraficosConsumoAlimentarByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetGraficosVocacionalByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetGraficosVocacionalByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return await Task.FromResult(Json(model));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Json(ex));
            }
        }
        public async Task<JsonResult> GetRelatorioVocacionalByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetRelatorioVocacionalByFilter(search);

                var model = new DashboardModel
                {
                    Dashboard = dashboard,

                };

                return Json(model);

            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
    }
}
