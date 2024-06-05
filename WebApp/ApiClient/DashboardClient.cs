﻿using NuGet.Protocol.Core.Types;
using WebApp.Dto;
using WebApp.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
	    private const string ResourceDashboard = "Dashboards";

        #region Main Methods


        public Task<DashboardDto?> GetGraficosTalentoByFilter(DashboardDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/GraficosTalento"));
            return GetFiltro(requestUrl, searchFilter);
        }
        public Task<DashboardDto?> GetGraficoPercDesempenhoFisicoMotorByFilter(DashboardDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/GraficoPercDesempenhoFisicoMotor"));
            return GetFiltro(requestUrl, searchFilter);
        }
        public Task<DashboardDto?> GetGraficosQualidadeVidaByFilter(DashboardDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/GetGraficosQualidadeVida"));
            return GetFiltro(requestUrl, searchFilter);
        }
        public Task<DashboardDto?> GetGraficosConsumoAlimentarByFilter(DashboardDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/GetGraficosConsumoAlimentar"));
            return GetFiltro(requestUrl, searchFilter);
        }
        public Task<DashboardDto?> GetGraficosVocacionalByFilter(DashboardDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/GetGraficosVocacional"));
            return GetFiltro(requestUrl, searchFilter);
        }
        public Task<DashboardDto?> GetGraficosSaudeByFilter(DashboardDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/GraficosSaude"));
            return GetFiltro(requestUrl, searchFilter);
        }
        public Task<DashboardDto?> GetGraficosSaudeBucalByFilter(DashboardDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/GraficosSaudeBucal"));
            return GetFiltro(requestUrl, searchFilter);
        }
        public Task<DashboardDto?> GetGraficosEtniaByFilter(DashboardDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/GraficosEtnia"));
            return GetFiltro(requestUrl, searchFilter);
        }
        public Task<DashboardDto?> GetGraficosDeficienciasByFilter(DashboardDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/GraficosDeficiencia"));
            return GetFiltro(requestUrl, searchFilter);
        }
        public Task<DashboardDto?> GetIndicadoresAlunosByFilter(DashboardDto searchFilter)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceDashboard}/Indicadores"));
			return GetFiltro(requestUrl, searchFilter);
		}
        public Task<DashboardDto?> GetControlePresencaByFilter(DashboardDto searchFilter)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceDashboard}/ControlePresenca"));
			return GetFiltro(requestUrl, searchFilter);
		}
        public Task<DashboardDto?> GetLaudosPeriodoByFilter(DashboardDto searchFilter)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceDashboard}/LaudosPeriodo"));
			return GetFiltro(requestUrl, searchFilter);
		}
        public Task<DashboardDto?> GetStatusLaudosByFilter(DashboardDto searchFilter)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceDashboard}/StatusLaudos"));
			return GetFiltro(requestUrl, searchFilter);
		}
        public Task<DashboardDto?> GetEvolutivoByFilter(DashboardDto searchFilter)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceDashboard}/Evolutivo"));
			return GetFiltro(requestUrl, searchFilter);
		}

        #endregion

        #region Methods


        #endregion


    }
}