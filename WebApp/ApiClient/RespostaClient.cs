using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Resposta Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceResposta = "Respostas";
        #region Main Methods

        /// <summary>
        /// inclusão de Resposta
        /// </summary>
        /// <param name="command">Objeto de inclusão de Resposta</param>
        /// <returns>Retorna o objeto de Resposta</returns>
        public Task<long> CreateResposta(RespostaModel.CreateUpdateRespostaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceResposta}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        ///  Alteração de Resposta
        /// </summary>
        /// <param name="id">Id de alteração de Resposta</param>
        /// <param name="command">Objeto de alteração de Resposta</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateResposta(int id, RespostaModel.CreateUpdateRespostaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceResposta}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// exclusão de Resposta
        /// </summary>
        /// <param name="id">Id de exclusao de Resposta</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteResposta(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceResposta}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Resposta
        /// </summary>
        /// <param name="id">Id de Resposta a ser buscada</param>
        /// <returns>Retorna o objeto de Resposta </returns>
        public RespostaDto GetRespostaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceResposta}/{id}"));
            return Get<RespostaDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Resposta cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Resposta</returns>
        public List<RespostaDto> GetRespostaAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceResposta}"));
            return Get<List<RespostaDto>>(requestUrl);
        }

        #endregion
    }
}