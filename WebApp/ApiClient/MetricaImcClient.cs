using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Metrica Imc Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceMetricaImc = "MetricasImc";

        #region Main Methods

        /// <summary>
        /// inclusão de Métrica Imc
        /// </summary>
        /// <param name="command">Objeto de inclusão de Métrica Imc</param>
        /// <returns>Retorna Id de novas Métrica Imc</returns>
        public Task<long> CreateMetricaImc(MetricaImcModel.CreateUpdateMetricaImcCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMetricaImc}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Métriica Imc
        /// </summary>
        /// <param name="id">Id de alteração de Métrica Imc</param>
        /// <param name="command">Objeto de alteração de Métrica Imc</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateMetricaImc(int id, MetricaImcModel.CreateUpdateMetricaImcCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMetricaImc}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Métrica Imc
        /// </summary>
        /// <param name="id">Id de exclusao de Métrica Imc</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteMetricaImc(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMetricaImc}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Métricas Imc
        /// </summary>
        /// <param name="id">Id da Métricas Imc a ser buscada</param>
        /// <returns>Retorna o objeto de Métricas Imc</returns>
        public MetricaImcDto GetMetricaImcById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMetricaImc}/{id}"));
            return Get<MetricaImcDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Métricas Imc cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Métricas Imc</returns>
        public List<MetricaImcDto> GetMetricasImcAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMetricaImc}"));
            return Get<List<MetricaImcDto>>(requestUrl);
        }

        #endregion
    }
}