using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Serie Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceSerie = "Series";
        #region Main Methods

        /// <summary>
        /// Inclusão de Série
        /// </summary>
        /// <param name="command">Objeto de inclusão de Série</param>
        /// <returns>Id de Serie inserido</returns>
        public Task<long> CreateSerie(SerieModel.CreateUpdateSerieCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSerie}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Série
        /// </summary>
        /// <param name="id">Id de alteração de Série</param>
        /// <param name="command">Objeto de alteração de Série</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateSerie(int id, SerieModel.CreateUpdateSerieCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSerie}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Série
        /// </summary>
        /// <param name="id">Id de Exclusão de Série</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteSerie(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSerie}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Série
        /// </summary>
        /// <param name="id">Id de Série a ser buscada</param>
        /// <returns>Retorna o objeto de Série</returns>
        public SerieDto GetSerieById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSerie}/{id}"));
            return Get<SerieDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Série cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Série</returns>
        public List<SerieDto> GetSerieAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSerie}"));
            return Get<List<SerieDto>>(requestUrl);
        }

        /// <summary>
        /// Busca uma lista de séries por localidade e etapa
        /// </summary>
        /// <param name="localidadeId">Id da localidade</param>
        /// <param name="etapaId">Id da etapa de ensino</param>
        /// <returns>Retorna um json com a lista de séries</returns>
        public List<SerieDto> GetSeriesByLocalidadeIdEtapaId(int localidadeId, int etapaId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSerie}/Localidade/{localidadeId}/Etapa/{etapaId}"));
            return Get<List<SerieDto>>(requestUrl);
        }

        /// <summary>
        /// Busca uma lista de turmas por localidade, etapa e série
        /// </summary>
        /// <param name="localidadeId">Id da localidade</param>
        /// <param name="etapaId">Id da etapa de ensino</param>
        /// <param name="serie">Série selecionada</param>
        /// <returns>Retorna um json com a lista de séries</returns>
        public List<SerieDto> GetTurmasByLocalidadeIdEtapaIdSerie(int localidadeId, int etapaId, string serie)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSerie}/Localidade/{localidadeId}/Etapa/{etapaId}/Serie/{serie}"));
            return Get<List<SerieDto>>(requestUrl);
        }

        /// <summary>
        /// Busca uma lista de turmas por localidade
        /// </summary>
        /// <param name="localidadeId">Id da localidade</param>
        /// <returns>Retorna um json com a lista de séries</returns>
        public List<SerieDto> GetTurmasByLocalidadeId(int localidadeId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSerie}/Localidade/{localidadeId}"));
            return Get<List<SerieDto>>(requestUrl);
        }

        #endregion
    }
}