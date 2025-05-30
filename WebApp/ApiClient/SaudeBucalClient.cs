using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Saiude Bucal Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceSaudeBucal = "SaudeBucais";

        #region Main Methods

        /// <summary>
        /// Inclusão de Saúde Bucal
        /// </summary>
        /// <param name="command">Objeto de inclusão de Saúde Bucal</param>
        /// <returns>Retorna o objeto de Saude Bucal</returns>
        public Task<long> CreateSaudeBucal(SaudeBucalModel.CreateUpdateSaudeBucalCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSaudeBucal}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Saúde Bucais
        /// </summary>
        /// <param name="id">Id de alteração de Saúde Bucal</param>
        /// <param name="command">Objeto de alteração de Saúde Bucal</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateSaudeBucal(int id, SaudeBucalModel.CreateUpdateSaudeBucalCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSaudeBucal}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Saúde Bucal
        /// </summary>
        /// <param name="id">Id de exclusão de Saude Bucal</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteSaudeBucal(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSaudeBucal}/{id}"));
            return Delete<bool>(requestUrl);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Busca todos as Saúde Bucais cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Saude Bucal</returns>
        public List<SaudeBucalDto> GetSaudeBucalAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSaudeBucal}"));
            return Get<List<SaudeBucalDto>>(requestUrl);
        }

        /// <summary>
        ///  busca uma única Saúde Bucal
        /// </summary>
        /// <param name="id">Id de Saúde Bucal a ser buscada</param>
        /// <returns>Retorna o objeto de Saúde Bucal</returns>
        public SaudeBucalDto GetSaudeBucalById(int? id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSaudeBucal}/{id}"));
            return Get<SaudeBucalDto>(requestUrl);

        }
        #endregion
    }
}