using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        private const string ResourceControleMaterialEstoqueSaida = "ControlesMateriaisEstoquesSaidas";

        #region Main Methods

        public Task<long> CreateControleMaterialEstoqueSaida(ControleMaterialEstoqueSaidaModel.CreateUpdateControleMaterialEstoqueSaidaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}"));
            return Post(requestUrl, command);
        }
        public Task<bool> UpdateControleMaterialEstoqueSaida(int id, ControleMaterialEstoqueSaidaModel.CreateUpdateControleMaterialEstoqueSaidaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}/{id}"));
            return Put(requestUrl, command);
        }

        public Task<bool> DeleteControleMaterialEstoqueSaida(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        public ControleMaterialEstoqueSaidaDto GetControleMaterialEstoqueSaidaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}/{id}"));
            return Get<ControleMaterialEstoqueSaidaDto>(requestUrl);
        }
        public List<ControleMaterialEstoqueSaidaDto> GetControlesMateriaisEstoquesSaidasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}"));
            return Get<List<ControleMaterialEstoqueSaidaDto>>(requestUrl);
        }
        public List<ControleMaterialEstoqueSaidaDto> GetControlesMateriaisEstoquesSaidasByMaterialId(int materialId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}/ControleMaterialEstoqueSaida/{materialId}"));
            return Get<List<ControleMaterialEstoqueSaidaDto>>(requestUrl);
        }
        #endregion
    }
}