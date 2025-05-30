using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Saúde Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceSaude = "Saudes";

        #region Main Methods

        /// <summary>
        /// Inclusão de Saúde
        /// </summary>
        /// <param name="command">Objeto de Inclusão de Saúde</param>
        /// <returns>Id de Saúde inserido</returns>
        public Task<long> CreateSaude(SaudeModel.CreateUpdateSaudeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSaude}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Saúde
        /// </summary>
        /// <param name="id">Id de alteração de Saúde</param>
        /// <param name="command">Objeto de alteração de Saúde</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateSaude(int id, SaudeModel.CreateUpdateSaudeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSaude}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Saúde
        /// </summary>
        /// <param name="id">Id de exclusão de Saúde</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteSaude(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSaude}/{id}"));
            return Delete<bool>(requestUrl);
        }


        #endregion

        #region Methods

        /// <summary>
        /// busca uma única Saúde
        /// </summary>
        /// <param name="id">Id de Saúde a ser buscada</param>
        /// <returns>Retorna o objeto de Saúde</returns>
        public SaudeDto GetSaudeById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSaude}/{id}"));
            return Get<SaudeDto>(requestUrl);
        }

        /// <summary>
        /// Todas as Saúde cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Saúde</returns>
        public List<SaudeDto> GetSaudeAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceSaude}"));
            return Get<List<SaudeDto>>(requestUrl);
        }

        #endregion
    }
}