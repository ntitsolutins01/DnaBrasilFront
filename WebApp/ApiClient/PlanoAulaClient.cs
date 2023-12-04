using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {

        private const string ResourcePlanoAula = "PlanosAulas";

        #region Main Methods

        public Task<long> CreatePlanoAula(PlanoAulaModel.CreateUpdatePlanoAulaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePlanoAula}"));
            return Post(requestUrl, command);
        }

        public Task<bool> UpdatePlanoAula(int id, PlanoAulaModel.CreateUpdatePlanoAulaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePlanoAula}/{id}"));
            return Put(requestUrl, command);
        }

        public Task<bool> DeletePlanoAula(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePlanoAula}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        public PlanoAulaDto GetPlanoAulaById(string id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePlanoAula}/{id}"));
            return Get<PlanoAulaDto>(requestUrl);
        }
        public List<PlanoAulaDto> GetPlanosAulasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePlanoAula}"));
            return Get<List<PlanoAulaDto>>(requestUrl);
        }

        #endregion

    }
}