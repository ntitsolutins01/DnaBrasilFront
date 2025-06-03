using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        private const string ResourceGrupoMaterial = "GruposMateriais";

        #region Main Methods

        public Task<long> CreateGrupoMaterial(GrupoMaterialModel.CreateUpdateGrupoMaterialCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceGrupoMaterial}"));
            return Post(requestUrl, command);
        }
        public Task<bool> UpdateGrupoMaterial(int id, GrupoMaterialModel.CreateUpdateGrupoMaterialCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceGrupoMaterial}/{id}"));
            return Put(requestUrl, command);
        }

        public Task<bool> DeleteGrupoMaterial(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceGrupoMaterial}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        public GrupoMaterialDto GetGrupoMaterialById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceGrupoMaterial}/{id}"));
            return Get<GrupoMaterialDto>(requestUrl);
        }
        public List<GrupoMaterialDto> GetGruposMateriaisAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceGrupoMaterial}"));
            return Get<List<GrupoMaterialDto>>(requestUrl);
        }

        #endregion
    }
}