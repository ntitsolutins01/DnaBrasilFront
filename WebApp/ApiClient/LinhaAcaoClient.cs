using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Linha Açao Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceLinhaAcao = "LinhasAcoes";

        #region Main Methods

        /// <summary>
        /// Inclusão da Linha de açao
        /// </summary>
        /// <param name="command">Objeto de inclusão da Linha de açao</param>
        /// <returns>Id de Linha de Açao inserido</returns>
        public Task<long> CreateLinhaAcao(LinhaAcaoModel.CreateUpdateLinhaAcaoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLinhaAcao}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração da Linha de açao
        /// </summary>
        /// <param name="id">Id de alteração da linha de açao</param>
        /// <param name="command">Objeto de alteração da Linha de açao</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateLinhaAcao(int id, LinhaAcaoModel.CreateUpdateLinhaAcaoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLinhaAcao}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão da linha de açao
        /// </summary>
        /// <param name="id">Id de exclusao da linha de açao</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteLinhaAcao(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLinhaAcao}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Linha de açao
        /// </summary>
        /// <param name="id">Id da Linha de açao a ser buscada</param>
        /// <returns>Retorna o objeto da Linha de Açao</returns>
        public LinhaAcaoDto GetLinhaAcaoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLinhaAcao}/{id}"));
            return Get<LinhaAcaoDto>(requestUrl);
        }

        /// <summary>
        /// busca todas as Linhas de Açoes cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Linha de Açao</returns>
        public List<LinhaAcaoDto> GetLinhasAcoesAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLinhaAcao}"));
            return Get<List<LinhaAcaoDto>>(requestUrl);
        }

        #endregion
    }
}