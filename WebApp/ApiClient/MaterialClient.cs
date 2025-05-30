using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        private const string ResourceMaterial = "Materiais";

        #region Main Methods

        public Task<long> CreateMaterial(MaterialModel.CreateUpdateMaterialCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}"));
            return Post(requestUrl, command);
        }
        public Task<bool> UpdateMaterial(int id, MaterialModel.CreateUpdateMaterialCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}/{id}"));
            return Put(requestUrl, command);
        }

        public Task<bool> DeleteMaterial(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        public MaterialDto GetMaterialById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}/{id}"));
            return Get<MaterialDto>(requestUrl);
        }
        public List<MaterialDto> GetMateriaisAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}"));
            return Get<List<MaterialDto>>(requestUrl);
        }
        public List<MaterialDto> GetMateriaisByTipoMaterialId(int tipoMaterialId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}/TipoMaterial/{tipoMaterialId}"));
            return Get<List<MaterialDto>>(requestUrl);
        }
        public Task<MateriaisFilterDto?> GetMateriaisByFilter(MateriaisFilterDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}/Filter"));
            return GetFiltro(requestUrl, searchFilter);
        }
        #endregion
    }
}