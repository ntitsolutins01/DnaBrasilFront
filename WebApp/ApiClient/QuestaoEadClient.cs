using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        private const string ResourceQuestaoEad = "QuestoesEad";
        #region Main Methods

        public Task<long> CreateQuestaoEad(QuestaoEadModel.CreateUpdateQuestaoEadCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}"));
            return Post(requestUrl, command);
        }
        public Task<bool> UpdateQuestaoEad(int id, QuestaoEadModel.CreateUpdateQuestaoEadCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}/{id}"));
            return Put(requestUrl, command);
        }

        public Task<bool> DeleteQuestaoEad(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        public QuestaoEadDto GetQuestaoEadById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}/QuestaoEad/{id}"));
            return Get<QuestaoEadDto>(requestUrl);
        }
        public List<QuestaoEadDto> GetQuestaoEadAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}"));
            return Get<List<QuestaoEadDto>>(requestUrl);
        }
        public List<QuestaoEadDto> GetQuestaoEadByTipoLaudo(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestaoEad}/TipoLaudo/{id}"));
            return Get<List<QuestaoEadDto>>(requestUrl);
        }

        #endregion
    }
}