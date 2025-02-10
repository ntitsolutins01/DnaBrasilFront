using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Dto;
using WebApp.Enumerators;
using WebApp.Factory;
using WebApp.Identity;
using WebApp.Models;
using WebApp.Utility;

namespace WebApp.Controllers
{
    [Authorize(Policy = ModuloAccess.DashboardEad)]
    public class DashboardEadController : BaseController
    {
        private readonly ILogger<DashboardEadController> _logger;

        private readonly IOptions<UrlSettings> _appSettings;


        public DashboardEadController(ILogger<DashboardEadController> logger, IOptions<UrlSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
            ApplicationSettings.WebApiUrl = _appSettings.Value.WebApiBaseUrl;
        }
        public async Task<IActionResult> Index(IFormCollection collection)
        {
            //var usu = User?.Identity.Name;
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
            //var email = User.FindFirstValue(ClaimTypes.Email);
            //var token = HttpContext.Request.Cookies["token"];
            //ApplicationSettings.Token = HttpContext.Request.Cookies["token"];

            var searchFilter = new DashboardEadDto
            {
                FomentoId = collection["ddlFomento"].ToString(),
                Estado = collection["ddlEstado"].ToString(),
                MunicipioId = collection["ddlMunicipio"].ToString(),
                LocalidadeId = collection["ddlLocalidade"].ToString(),
                TipoCursoId = collection["ddlTipoCurso"].ToString(),
                CursoId = collection["ddlCurso"].ToString()
            };

            var dashboardEad = new DashboardEadDto();

            var fomentos = new SelectList(ApiClientFactory.Instance.GetFomentoAll(), "Id", "Nome", dashboardEad.FomentoId);
            var estados = new SelectList(ApiClientFactory.Instance.GetEstadosAll(), "Sigla", "Nome", dashboardEad.Estado);
            var tipoCurso = new SelectList(ApiClientFactory.Instance.GetTipoCursosAll(), "Id", "Nome");

            SelectList municipios = null;

            if (!string.IsNullOrEmpty(dashboardEad.Estado))
            {
                municipios = new SelectList(ApiClientFactory.Instance.GetMunicipiosByUf(dashboardEad.Estado), "Id", "Nome", dashboardEad.MunicipioId);
            }
            SelectList localidades = null;

            if (!string.IsNullOrEmpty(dashboardEad.LocalidadeId))
            {
                localidades = new SelectList(ApiClientFactory.Instance.GetLocalidadeByMunicipio(dashboardEad.MunicipioId), "Id", "Nome", dashboardEad.LocalidadeId);
            }

            var model = new DashboardEadModel
            {
                ListFomentos = fomentos,
                ListEstados = estados,
                DashboardEad = dashboardEad,
                ListMunicipios = municipios!,
                ListTipoCursos = tipoCurso,
                ListLocalidades = localidades!
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
        //public async Task<JsonResult> GetIndicadoresAlunosByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetIndicadoresAlunosByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return Json(model);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex);
        //    }
        //}
        //public async Task<JsonResult> GetControlePresencaByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetControlePresencaByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetLaudosPeriodoByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetLaudosPeriodoByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetStatusLaudosByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetStatusLaudosByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetEvolutivoByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        //var DashboardEad = await ApiClientFactory.Instance.GetEvolutivoByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            //DashboardEad = DashboardEad,
        //            DashboardEad = search,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetGraficosSaudeByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetGraficosSaudeByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetGraficosSaudeBucalByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetGraficosSaudeBucalByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetGraficosEtniaByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetGraficosEtniaByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetGraficosDeficienciasByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetGraficosDeficienciasByFilter(search);

        //        List<string> listPercDeficienciaCategorias = new List<string>();

        //        var listPercDeficiencia = new List<DataGrafico>();

        //        foreach (var item in DashboardEad.ListTotalizadorDeficiencia.PercDeficiencia)
        //        {
        //            listPercDeficiencia.Add(new DataGrafico()
        //            {
        //                name = item.Key,
        //                y = item.Value,
        //                z = 50
        //            });

        //            listPercDeficienciaCategorias.Add(new string(item.Key));
        //        }

        //        var listValorDeficienciaMasc = new List<DataGrafico>();

        //        foreach (var item in DashboardEad.ListTotalizadorDeficiencia.ValorTotalizadorDeficienciaMasculino!)
        //        {
        //            listValorDeficienciaMasc.Add(new DataGrafico()
        //            {
        //                name = item.Key,
        //                y = item.Value,
        //                z = 50
        //            });

        //        }
        //        var listValorDeficienciaFem = new List<DataGrafico>();

        //        foreach (var item in DashboardEad.ListTotalizadorDeficiencia.ValorTotalizadorDeficienciaFeminino!)
        //        {
        //            listValorDeficienciaFem.Add(new DataGrafico()
        //            {
        //                name = item.Key,
        //                y = item.Value,
        //                z = 50
        //            });

        //        }

        //        DashboardEad.ListPercDeficiencia = listPercDeficiencia;
        //        DashboardEad.ListPercDeficienciaCategorias = listPercDeficienciaCategorias;
        //        DashboardEad.ListValorDeficienciaMasc = listValorDeficienciaMasc;
        //        DashboardEad.ListValorDeficienciaFem = listValorDeficienciaFem;

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetGraficosTalentoByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetGraficosTalentoByFilter(search);

        //        List<string> listPercTalentoCategorias = new List<string>();

        //        var listPercTalento = new List<DataGrafico>();

        //        foreach (var item in DashboardEad.ListTotalizadorTalento.PercTalento)
        //        {
        //            listPercTalento.Add(new DataGrafico()
        //            {
        //                name = item.Key,
        //                y = item.Value,
        //                z = 50
        //            });

        //            listPercTalentoCategorias.Add(new string(item.Key));
        //        }

        //        var listValorTalentoMasc = new List<DataGrafico>();

        //        foreach (var item in DashboardEad.ListTotalizadorTalento.ValorTotalizadorTalentoMasculino!)
        //        {
        //            listValorTalentoMasc.Add(new DataGrafico()
        //            {
        //                name = item.Key,
        //                y = item.Value,
        //                z = 50
        //            });

        //        }
        //        var listValorTalentoFem = new List<DataGrafico>();

        //        foreach (var item in DashboardEad.ListTotalizadorTalento.ValorTotalizadorTalentoFeminino!)
        //        {
        //            listValorTalentoFem.Add(new DataGrafico()
        //            {
        //                name = item.Key,
        //                y = item.Value,
        //                z = 50
        //            });

        //        }

        //        DashboardEad.ListPercTalento = listPercTalento;
        //        DashboardEad.ListPercTalentoCategorias = listPercTalentoCategorias;
        //        DashboardEad.ListValorTalentoMasc = listValorTalentoMasc;
        //        DashboardEad.ListValorTalentoFem = listValorTalentoFem;

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetGraficoPercDesempenhoFisicoMotorByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetGraficoPercDesempenhoFisicoMotorByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetGraficosQualidadeVidaByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetGraficosQualidadeVidaByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetGraficosConsumoAlimentarByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetGraficosConsumoAlimentarByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetGraficosVocacionalByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetGraficosVocacionalByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return await Task.FromResult(Json(model));

        //    }
        //    catch (Exception ex)
        //    {
        //        return await Task.FromResult(Json(ex));
        //    }
        //}
        //public async Task<JsonResult> GetRelatorioVocacionalByFilter([FromBody] DashboardEadDto search)
        //{
        //    try
        //    {
        //        var DashboardEad = await ApiClientFactory.Instance.GetRelatorioVocacionalByFilter(search);

        //        var model = new DashboardEadModel
        //        {
        //            DashboardEad = DashboardEad,

        //        };

        //        return Json(model);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex);
        //    }
        //}
    }
}
