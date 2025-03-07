using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Tipo de Material Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceTipoMaterial = "TiposMateriais";

        #region Main Methods

        /// <summary>
        /// Inclusão do Tipo de Material
        /// </summary>
        /// <param name="command">Objeto de inclusão do Tipo de Material</param>
        /// <returns>Retorna id do Tipo de Material inserido</returns>
        public Task<long> CreateTipoMaterial(TipoMaterialModel.CreateUpdateTipoMaterialCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração do Tipo de Material
        /// </summary>
        /// <param name="id">Id de alteração do Tipo de Material</param>
        /// <param name="command">Objeto de alteração do Tipo de Material</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateTipoMaterial(int id, TipoMaterialModel.CreateUpdateTipoMaterialCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão do Tipo de Material
        /// </summary>
        /// <param name="id">Id de exclusão do Tipo de Material</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteTipoMaterial(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca Tipo de Material por id
        /// </summary>
        /// <param name="id">Id do Tipo de Material a ser buscada</param>
        /// <returns>Retorna o objeto do Tipo de Material</returns>
        public TipoMaterialDto GetTipoMaterialById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}/TipoMaterial/{id}"));
            return Get<TipoMaterialDto>(requestUrl);
        }

        /// <summary>
        /// Busca Todos os Tipos de Materias cadastrados
        /// </summary>
        /// <returns>Retorna a Lista do Tipo de Material</returns>
        public List<TipoMaterialDto> GetTiposMateriaisAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}"));
            return Get<List<TipoMaterialDto>>(requestUrl);
        }

        /// <summary>
        /// Busca Tipos de Materiais por id do Grupo de Material
        /// </summary>
        /// <param name="grupoMaterialId">Id do grupo de material</param>
        /// <returns>Retorna a um grupo de material</returns>
        public List<TipoMaterialDto> GetTiposMateriaisByGrupoMaterialId(int grupoMaterialId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoMaterial}/GrupoMaterial/{grupoMaterialId}"));
            return Get<List<TipoMaterialDto>>(requestUrl);
        }
        #endregion
    }
}