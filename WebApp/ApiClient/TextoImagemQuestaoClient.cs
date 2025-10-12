using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// TextoImagemQuestao Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceTextoImagemQuestao = "TextosQuestoes";

        #region Main Methods
        /// <summary>
        /// Inclusão de TextoImagemQuestao
        /// </summary>
        /// <param name="command">Objeto para inclusão do TextoImagemQuestao</param>
        /// <returns>Id do curso inserido</returns>
        public Task<long> CreateTextoImagemQuestao(TextoImagemQuestaoModel.CreateUpdateTextoImagemQuestaoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTextoImagemQuestao}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de TextoImagemQuestao
        /// </summary>
        /// <param name="id">Id de alteração de TextoImagemQuestao</param>
        /// <param name="command">Objeto de alteração de TextoImagemQuestao</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateTextoImagemQuestao(int id, TextoImagemQuestaoModel.CreateUpdateTextoImagemQuestaoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTextoImagemQuestao}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de TextoImagemQuestao
        /// </summary>
        /// <param name="id">Id de exclusão de TextoImagemQuestao</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteTextoImagemQuestao(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTextoImagemQuestao}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único TextoImagemQuestao
        /// </summary>
        /// <param name="id">Id de TextoImagemQuestao a ser buscado</param>
        /// <returns>Retorna o objeto de TextoImagemQuestao</returns>
        public TextoImagemQuestaoDto GetTextoImagemQuestaoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTextoImagemQuestao}/{id}"));
            return Get<TextoImagemQuestaoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os TextosImagensQuestoes cadastrados
        /// </summary>
        /// <returns>Retorna a lista de TextosImagensQuestoes</returns>
        public List<TextoImagemQuestaoDto> GetTextosImagensQuestoesAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTextoImagemQuestao}"));
            return Get<List<TextoImagemQuestaoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todos os TextosImagensQuestoes por id 
        /// </summary>
        /// <param name="questaoEadId">Id do tipo de curso</param>
        /// <returns>Retorna a lista por TextoImagemQuestao id</returns>
        public List<TextoImagemQuestaoDto> GetTextosImagensQuestoesAllByQuestaoEadId(int questaoEadId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTextoImagemQuestao}/QuestaoEad/{questaoEadId}"));
            return Get<List<TextoImagemQuestaoDto>>(requestUrl);
        }

        #endregion
    }
}