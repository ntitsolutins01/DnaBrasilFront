using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Escolaridade Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceEscolaridade = "Escolaridades";

        #region Main Methods

        /// <summary>
        /// Inclusão de Escolaridade 
        /// </summary>
        /// <param name="command">Objeto de inclusão de Escolaridade</param>
        /// <returns>Id de Escolaridade inserido</returns>
        public Task<long> CreateEscolaridade(EscolaridadeModel.CreateUpdateEscolaridadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEscolaridade}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Escolaridade
        /// </summary>
        /// <param name="id">Id de alteração de Escolaridade</param>
        /// <param name="command">Objeto de alteração de Escolaridade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateEscolaridade(int id, EscolaridadeModel.CreateUpdateEscolaridadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEscolaridade}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Escolaridade
        /// </summary>
        /// <param name="id">Id de exclusao de Escolaridade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteEscolaridade(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEscolaridade}/{id}"));
            return Delete<bool>(requestUrl);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Busca Escolaridade por id
        /// </summary>
        /// <param name="id">Id de Escolaridade a ser buscado</param>
        /// <returns>Retorna o objeto de Escolaridade</returns>
        public EscolaridadeDto GetEscolaridadeById(string id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEscolaridade}/{id}"));
            return Get<EscolaridadeDto>(requestUrl);
        }

        /// <summary>
        /// Busca Escolaridades cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Escolaridade</returns>
        public List<EscolaridadeDto> GetEscolaridadeAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEscolaridade}"));
            return Get<List<EscolaridadeDto>>(requestUrl);
        }
        #endregion
    }
}