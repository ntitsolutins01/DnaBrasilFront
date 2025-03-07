using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Comunidade Client
    /// </summary>
    public partial class DnaApiClient
    {
	    private const string ResourceComunidade = "Comunidades";

        #region Main Methods

        /// <summary>
        /// Inclusão de Comunidade
        /// </summary>
        /// <param name="command">Objeto para inclusão de Comunidade</param>
        /// <returns>Id de Comunidade inserido</returns>
        public Task<long> CreateComunidade (ComunidadeModel.CreateUpdateComunidadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceComunidade }"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Comunidade
        /// </summary>
        /// <param name="id">Id de alteração de Comunidade</param>
        /// <param name="command">Objeto de alteração de Comunidade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateComunidade (int id, ComunidadeModel.CreateUpdateComunidadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceComunidade }/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Comunidade
        /// </summary>
        /// <param name="id">Id de exclusão de Comunidade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteComunidade (int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceComunidade }/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Comunidade
        /// </summary>
        /// <param name="id">Id de Comunidade a ser buscado</param>
        /// <returns>Retorna o objeto de Comunidade</returns>
        public ComunidadeDto GetComunidadeById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceComunidade}/{id}"));
            return Get<ComunidadeDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Comunidades cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Comunidade</returns>
        public List<ComunidadeDto> GetComunidadesAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceComunidade}"));
            return Get<List<ComunidadeDto>>(requestUrl);
        }

        #endregion
    }
}