using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Localidade Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceLocalidade = "Localidades";

        #region Main Methods

        /// <summary>
        /// Inclusão de Localidade
        /// </summary>
        /// <param name="command">Objeto de inclusão de Localidade</param>
        /// <returns>Id de Localidade inserido</returns>
        public Task<long> CreateLocalidade(LocalidadeModel.CreateUpdateLocalidadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLocalidade}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Localidade
        /// </summary>
        /// <param name="id">Id de alteração de localidade</param>
        /// <param name="command">Objeto de alteração de Localidade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateLocalidade(int id, LocalidadeModel.CreateUpdateLocalidadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLocalidade}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Localidade
        /// </summary>
        /// <param name="id">Id de exclusao de Localidade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteLocalidade(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLocalidade}/{id}"));
            return Delete<bool>(requestUrl);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Busca todas as Localidade Cadastradas
        /// </summary>
        /// <returns>Retorna a Lista de Localidade</returns>
        public List<LocalidadeDto> GetLocalidadeAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLocalidade}"));
            return Get<List<LocalidadeDto>>(requestUrl);
        }

        /// <summary>
        /// Busca uma única Localidade
        /// </summary>
        /// <param name="id">Id de Localidade a ser buscada</param>
        /// <returns>Retorna o objeto de uma  Localidade</returns>
        public LocalidadeDto GetLocalidadeById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLocalidade}/{id}"));
            return Get<LocalidadeDto>(requestUrl);
        }

        /// <summary>
        /// Busca Localidade por Municipio
        /// </summary>
        /// <param name="id">Id de Localidade por Municipio</param>
        /// <returns>retorna uma lista de Localidade</returns>
        public List<LocalidadeDto> GetLocalidadeByMunicipioId(string id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLocalidade}/Municipio/{id}"));
            return Get<List<LocalidadeDto>>(requestUrl);
        }

        /// <summary>
        /// Busca Localidade por Fomento
        /// </summary>
        /// <param name="id">id de Localidade por Fomento</param>
        /// <returns>retorna a uma Lista de Localidade</returns>
        public List<LocalidadeDto> GetLocalidadeByFomento(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLocalidade}/Fomento/{id}"));
            return Get<List<LocalidadeDto>>(requestUrl);
        }

        #endregion
    }
}