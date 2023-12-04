using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        private const string ResourceFomento = "Fomentos";
        #region Main Methods

        public Task<long> CreateFomento(FomentoModel.CreateUpdateFomentoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}"));
            return Post(requestUrl, command);
        }
        public Task<bool> UpdateFomento(int id, FomentoModel.CreateUpdateFomentoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}/{id}"));
            return Put(requestUrl, command);
        }

        public Task<bool> DeleteFomento(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        public FomentoDto GetFomentoById(string id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}/{id}"));
            return Get<FomentoDto>(requestUrl);
        }
        public List<FomentoDto> GetFomentoAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}"));
            return Get<List<FomentoDto>>(requestUrl);
        }

        #endregion
    }
}