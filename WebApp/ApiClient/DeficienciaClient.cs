using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Deficinecia Client
    /// </summary>
    public partial class DnaApiClient
    {

        private const string ResourceDeficiencia = "Deficiencias";

        #region Main Methods

        /// <summary>
        /// Inclusão da Deficiencia
        /// </summary>
        /// <param name="command">Objeto de inclusão da Deficiencia</param>
        /// <returns>Id da Deficiencia inserido</returns>
        public Task<long> CreateDeficiencia(DeficienciaModel.CreateUpdateDeficienciaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDeficiencia}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração da Deficiencia
        /// </summary>
        /// <param name="id">Id de alteração da Deficiencia</param>
        /// <param name="command">Objeto de alteração da Deficiencia</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateDeficiencia(int id, DeficienciaModel.CreateUpdateDeficienciaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDeficiencia}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão da Deficiencia
        /// </summary>
        /// <param name="id">Id de exclusao da Deficiencia</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteDeficiencia(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDeficiencia}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca todas as Deficiencia cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Deficiencia</returns>
        public List<DeficienciaDto> GetDeficienciaAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDeficiencia}"));
            return Get<List<DeficienciaDto>>(requestUrl);
        }

        /// <summary>
        /// Busca uma única Deficiencia
        /// </summary>
        /// <param name="id">Id da Deficiencia a ser buscada</param>
        /// <returns>Retorna o objeto da Deficiencia</returns>
        public DeficienciaDto GetDeficienciaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDeficiencia}/{id}"));
            return Get<DeficienciaDto>(requestUrl);

        }
        #endregion
    }
}