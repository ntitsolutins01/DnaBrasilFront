using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// RespostaEad Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceRespostaEad = "RespostasEad";

        #region Main Methods
        /// <summary>
        /// Inclusão de RespostaEad
        /// </summary>
        /// <param name="command">Objeto para inclusão do RespostaEad</param>
        /// <returns>Id do curso inserido</returns>
        public Task<long> CreateRespostaEad(RespostaEadModel.CreateUpdateRespostaEadCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceRespostaEad}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de RespostaEad
        /// </summary>
        /// <param name="id">Id de alteração de RespostaEad</param>
        /// <param name="command">Objeto de alteração de RespostaEad</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateRespostaEad(int id, RespostaEadModel.CreateUpdateRespostaEadCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceRespostaEad}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de RespostaEad
        /// </summary>
        /// <param name="id">Id de exclusão de RespostaEad</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteRespostaEad(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceRespostaEad}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único RespostaEad
        /// </summary>
        /// <param name="id">Id de RespostaEad a ser buscado</param>
        /// <returns>Retorna o objeto de RespostaEad</returns>
        public RespostaEadDto GetRespostaEadById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceRespostaEad}/{id}"));
            return Get<RespostaEadDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os RespostasEad cadastrados
        /// </summary>
        /// <returns>Retorna a lista de RespostasEad</returns>
        public List<RespostaEadDto> GetRespostasEadAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceRespostaEad}"));
            return Get<List<RespostaEadDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todos os RespostasEad por id 
        /// </summary>
        /// <param name="questaoEadId">Id do tipo de curso</param>
        /// <returns>Retorna a lista por RespostaEad id</returns>
        public List<RespostaEadDto> GetRespostasEadByQuestaoEadId(int questaoEadId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceRespostaEad}/QuestaoEad/{questaoEadId}"));
            return Get<List<RespostaEadDto>>(requestUrl);
        }

        #endregion
    }
}