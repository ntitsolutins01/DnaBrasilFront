using WebApp.Dto;

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
        public Task<DashboardDto?> GetRelatorioVocacionalByFilter(DashboardDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/GetRelatorioVocacional"));
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
                $"{ResourceDashboard}/2"));
            return GetFiltro(requestUrl, searchFilter);
        }
        public Task<DashboardDto?> GetGraficosEducacionalByFilter(DashboardDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/GraficosEducacional"));
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
        public Task<DashboardEadDto?> GetIndicadoresEadByFilter(DashboardEadDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDashboard}/IndicadoresEad"));
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