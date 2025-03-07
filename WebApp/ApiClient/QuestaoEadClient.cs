using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Questão Ead Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceQuestaoEad = "QuestoesEad";

        #region Main Methods

        /// <summary>
        /// Inclusão de Questão Ead
        /// </summary>
        /// <param name="command">Objeto de inclusão da Questão Ead</param>
        /// <returns>Retorna id da Questão Ead</returns>
        public Task<long> CreateQuestaoEad(QuestaoEadModel.CreateUpdateQuestaoEadCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração da Questão Ead
        /// </summary>
        /// <param name="id">Id de alteração da Questão Ead</param>
        /// <param name="command">Objeto de alteração da Questão Ead</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateQuestaoEad(int id, QuestaoEadModel.CreateUpdateQuestaoEadCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão da Questão Ead
        /// </summary>
        /// <param name="id">Id de exclusão da Questão Ead</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteQuestaoEad(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca Questão Ead por id
        /// </summary>
        /// <param name="id">Id da Questão Ead por id a ser buscada</param>
        /// <returns>Retorna o objeto de uma Questão Ead por id</returns>
        public QuestaoEadDto GetQuestaoEadById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}/QuestaoEad/{id}"));
            return Get<QuestaoEadDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Questões Ead cadastradas
        /// </summary>
        /// <returns>Retorna a Lista de Questões Ead</returns>
        public List<QuestaoEadDto> GetQuestaoEadAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}"));
            return Get<List<QuestaoEadDto>>(requestUrl);
        }

        /// <summary>
        /// Busca  Questão Ead por Tipo de Laudo
        /// </summary>
        /// <param name="id">Id da Questão Ead por Tipo de Laudo</param>
        /// <returns>Retorna uma lista de Questão Ead por Tipo de Laudo</returns>
        public List<QuestaoEadDto> GetQuestaoEadByTipoLaudo(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}/TipoLaudo/{id}"));
            return Get<List<QuestaoEadDto>>(requestUrl);
        }

        #endregion
    }
}