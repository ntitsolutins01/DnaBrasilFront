using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        private const string ResourceInventario = "Inventarios";

        #region Main Methods

        public Task<long> CreateInventario(InventarioModel.CreateUpdateInventarioCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceInventario}"));
            return Post(requestUrl, command);
        }
        public Task<bool> UpdateInventario(int id, InventarioModel.CreateUpdateInventarioCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceInventario}/{id}"));
            return Put(requestUrl, command);
        }

        public Task<bool> DeleteInventario(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceInventario}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        public InventarioDto GetInventarioById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceInventario}/{id}"));
            return Get<InventarioDto>(requestUrl);
        }
        public List<InventarioDto> GetInventariosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceInventario}"));
            return Get<List<InventarioDto>>(requestUrl);
        }
        public List<InventarioDto> GetInventariosByMaterialId(int materialId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceInventario}/Material/{materialId}"));
            return Get<List<InventarioDto>>(requestUrl);
        }
        public List<InventarioDto> GetInventariosByLocalidadeId(int localidadeId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceInventario}/Localidade/{localidadeId}"));
            return Get<List<InventarioDto>>(requestUrl);
        }
        public Task<InventariosFilterDto?> GetInventariosByFilter(InventariosFilterDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceInventario}/Filter"));
            return GetFiltro(requestUrl, searchFilter);
        }
        #endregion
    }
}