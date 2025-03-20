using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        private const string ResourceArquivosInventario = "ArquivosInventarios";

        #region Main Methods

        public Task<long> CreateArquivosInventario(ArquivosInventarioModel.CreateUpdateArquivosInventarioCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceArquivosInventario}"));
            return Post(requestUrl, command);
        }
        public Task<bool> UpdateArquivosInventario(int id, ArquivosInventarioModel.CreateUpdateArquivosInventarioCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceArquivosInventario}/{id}"));
            return Put(requestUrl, command);
        }

        public Task<bool> DeleteArquivosInventario(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceArquivosInventario}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        public ArquivosInventarioDto GetArquivosInventarioById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceArquivosInventario}/{id}"));
            return Get<ArquivosInventarioDto>(requestUrl);
        }
        public List<ArquivosInventarioDto> GetArquivosInventariosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceArquivosInventario}"));
            return Get<List<ArquivosInventarioDto>>(requestUrl);
        }
        public List<ArquivosInventarioDto> GetArquivosInventariosByInventarioId(int inventarioId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceArquivosInventario}/Inventario/{inventarioId}"));
            return Get<List<ArquivosInventarioDto>>(requestUrl);
        }
        #endregion
    }
}