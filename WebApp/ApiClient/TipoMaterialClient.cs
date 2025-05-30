using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        private const string ResourceTipoMaterial = "TiposMateriais";

        #region Main Methods

        public Task<long> CreateTipoMaterial(TipoMaterialModel.CreateUpdateTipoMaterialCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}"));
            return Post(requestUrl, command);
        }
        public Task<bool> UpdateTipoMaterial(int id, TipoMaterialModel.CreateUpdateTipoMaterialCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}/{id}"));
            return Put(requestUrl, command);
        }

        public Task<bool> DeleteTipoMaterial(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        public TipoMaterialDto GetTipoMaterialById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}/{id}"));
            return Get<TipoMaterialDto>(requestUrl);
        }
        public List<TipoMaterialDto> GetTiposMateriaisAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}"));
            return Get<List<TipoMaterialDto>>(requestUrl);
        }
        public List<TipoMaterialDto> GetTiposMateriaisByGrupoMaterialId(int grupoMaterialId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}/GrupoMaterial/{grupoMaterialId}"));
            return Get<List<TipoMaterialDto>>(requestUrl);
        }
        #endregion
    }
}