using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Ranking Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceRanking = "Rankings";

        #region Main Methods

        /// <summary>
        /// Inclusão de Ranking
        /// </summary>
        /// <param name="command">Objeto para inclusão de Ranking</param>
        /// <returns>Id do curso Ranking</returns>
        public Task<long> CreateRanking(RankingModel.CreateUpdateRankingCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceRanking}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        ///  Alteração de Ranking
        /// </summary>
        /// <param name="id">Id de alteração de Ranking</param>
        /// <param name="command">Objeto de alteração de Ranking </param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateRanking(int id, RankingModel.CreateUpdateRankingCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceRanking}/{id}"));
            return Put(requestUrl, command);
        }


        /// <summary>
        /// Exclusão de  Ranking
        /// </summary>
        /// <param name="id">Id de exclusão de Ranking</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteRanking(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceRanking}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Ranking
        /// </summary>
        /// <param name="id">Id de Ranking a ser buscado</param>
        /// <returns>Retorna o objeto de Ranking</returns>
        public RankingDto GetRankingById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceRanking}/Ranking/{id}"));
            return Get<RankingDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Rankings cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Ranking</returns>
        public List<RankingDto> GetRankingsAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceRanking}"));
            return Get<List<RankingDto>>(requestUrl);
        }

        #endregion
    }
}