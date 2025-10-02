using log4net;
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
        #region Parametros
        private readonly ILog _logger;
        private readonly IOptions<UrlSettings> _appSettings;
        #endregion

        #region Constructor

        public DashboardController(IOptions<UrlSettings> appSettings, IWebHostEnvironment host, ILog logger)
        {
            _logger = logger;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
        }
        #endregion

        #region Main Methods

        /// <summary>
        /// Dashboard
        /// </summary>
        /// <param name="collection">Coleção de dados para inclusao de Curso</param>
        /// <returns>Retorna dados para montar os gráficos no dashboard</returns>
        public async Task<IActionResult> Index(IFormCollection collection)
        {
            try
            {
                _logger.Info($"Usuario Logado em Dashboard.Index User.Identity.Name : {User.Identity.Name}");

                var usuario = User?.Identity.Name;

                var usu = await ApiClientFactory.Instance.GetUsuarioByEmail(usuario);

                var dashboard = new DashboardDto();
                var dashboardEad = new DashboardEadDto();

                var fomento = ApiClientFactory.Instance.GetFomentoByLocalidadeId(Convert.ToInt32(usu.LocalidadeId));
                var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentosAll(), "Id", "Nome", fomento.Id);

                var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", usu.Uf);

                var tipoCurso = new SelectList(ApiClientFactory.Instance.GetTipoCursosAll(), "Id", "Nome");

                SelectList municipios = null;

                if (!string.IsNullOrEmpty(usu.Uf))
                {
                    municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByFomentoId(fomento.Id), "Id", "Nome");
                }

                SelectList localidades = null;

                if (usu.MunicipioId != null)
                {
                    var resultLocalidades = ApiClientFactory.Instance.GetLocalidadeByMunicipioId(usu.MunicipioId.ToString());

                    if (resultLocalidades != null)
                        localidades = new SelectList(resultLocalidades, "Id", "Nome");
                }

                var model = new DashboardModel
                {
                    ListFomentos = fomentos,
                    ListEstados = estados,
                    Dashboard = dashboard,
                    DashboardEad = dashboardEad,
                    ListMunicipios = municipios!,
                    ListLocalidades = localidades!,
                    ListTipoCursos = tipoCurso,
                    IdPerfil = usu.Perfil.Id
                };

                model.Dashboard.StatusLaudos = new StatusLaudosDto();


                return View(model);

            }
            catch (Exception e)
            {
                _logger.Error($"Dashboard.Index: {e.StackTrace}");
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error",
                    message = e.Message,
                    stackTrace = e.StackTrace
                });
            }
            
        }
        #endregion

        #region Get Methods
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
        public async Task<JsonResult> GetIndicadoresEadByFilter([FromBody] DashboardEadDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetIndicadoresEadByFilter(search);

                var model = new DashboardModel
                {
                    DashboardEad = dashboard,

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
        public async Task<JsonResult> GetGraficosEducacionalByFilter([FromBody] DashboardDto search)
        {
            try
            {
                var dashboard = await ApiClientFactory.Instance.GetGraficosEducacionalByFilter(search);

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
        #endregion
    }
}
