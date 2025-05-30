using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Estrutura Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceEstrutura = "Estruturas";

        #region Main Methods

        /// <summary>
        /// Inclusão de Estrutura
        /// </summary>
        /// <param name="command">Objeto de inclusão da Estrutura</param>
        /// <returns>Id de Estrutura inserido</returns>
        public Task<long> CreateEstrutura(EstruturaModel.CreateUpdateEstruturaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEstrutura}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Estrutura
        /// </summary>
        /// <param name="id">Id de alteração da Estrutura</param>
        /// <param name="command">Objeto de alteração da Estrutura</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateEstrutura(int id, EstruturaModel.CreateUpdateEstruturaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEstrutura}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Estrutura
        /// </summary>
        /// <param name="id">Id de exclusao da Estrutura</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteEstrutura(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEstrutura}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Estrutura
        /// </summary>
        /// <param name="id">Id da Estrutura a ser buscada</param>
        /// <returns>Retorna o objeto da Estrutura</returns>
        public EstruturaDto GetEstruturaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEstrutura}/{id}"));
            return Get<EstruturaDto>(requestUrl);
        }

        /// <summary>
        ///  Busca todas as Estruturas cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Estruturas</returns>
        public List<EstruturaDto> GetEstruturasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEstrutura}"));
            return Get<List<EstruturaDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todas as estruturas por localidade
        /// </summary>
        /// <param name="id">Id de Estrutura da localidade</param>
        /// <returns>Retorna a lista de Estruturas</returns>
        public List<EstruturaDto> GetEstruturasByLocalidade(string id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEstrutura}/Localidade/{id}"));
            return Get<List<EstruturaDto>>(requestUrl);
        }
        #endregion


    }
}