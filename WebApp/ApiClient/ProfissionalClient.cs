using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {

        private const string ResourceProfissional = "Profissionais";

        #region Main Methods

        public Task<long> CreateProfissional(ProfissionalModel.CreateUpdateProfissionalCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceProfissional}"));
            return Post(requestUrl, command);
        }

        public Task<bool> UpdateProfissional(int id, ProfissionalModel.CreateUpdateProfissionalCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceProfissional}/{id}"));
            return Put(requestUrl, command);
        }

        public Task<bool> DeleteProfissional(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceProfissional}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        public ProfissionalDto GetProfissionalById(string id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceProfissional}/{id}"));
            return Get<ProfissionalDto>(requestUrl);
        }
        public List<ProfissionalDto> GetProfissionalAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceProfissional}"));
            return Get<List<ProfissionalDto>>(requestUrl);
        }

        #endregion

    }
}