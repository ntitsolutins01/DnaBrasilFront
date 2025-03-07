using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Material Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceMaterial = "Materiais";

        #region Main Methods

        /// <summary>
        /// Inclusão do Material
        /// </summary>
        /// <param name="command">Objeto de inclusão do Material</param>
        /// <returns>Retorna o id do Material inserido</returns>
        public Task<long> CreateMaterial(MaterialModel.CreateUpdateMaterialCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}"));
            return Post(requestUrl, command);
        }
        /// <summary>
        /// Alteração do Material
        /// </summary>
        /// <param name="id">Id de alteração do Material</param>
        /// <param name="command">Objeto de alteração do Material</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateMaterial(int id, MaterialModel.CreateUpdateMaterialCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> DeleteMaterial(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Exclusão do Material
        /// </summary>
        /// <param name="id">Id de exclusão do Material</param>
        /// <returns>Retorna true ou false</returns>
        public MaterialDto GetMaterialById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}/{id}"));
            return Get<MaterialDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Materiais cadastrados 
        /// </summary>
        /// <returns>Retorna a Lista de Material</returns>
        public List<MaterialDto> GetMateriaisAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}"));
            return Get<List<MaterialDto>>(requestUrl);
        }

        /// <summary>
        /// Busca Materiais por id de Material
        /// </summary>
        /// <param name="tipoMaterialId">Id do Tipo Material</param>
        /// <returns>Retorna o objeto de um Material</returns>
        public List<MaterialDto> GetMateriaisByTipoMaterialId(int tipoMaterialId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}/TipoMaterial/{tipoMaterialId}"));
            return Get<List<MaterialDto>>(requestUrl);
        }

        /// <summary>
        /// Busca Material por filtro
        /// </summary>
        /// <param name="searchFilter">Filtro de Busca</param>
        /// <returns>Retorna a lista de Material</returns>
        public Task<MateriaisFilterDto?> GetMateriaisByFilter(MateriaisFilterDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMaterial}/Filter"));
            return GetFiltro(requestUrl, searchFilter);
        }
        #endregion
    }
}