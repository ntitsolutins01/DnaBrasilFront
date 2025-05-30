using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Dependecia Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceDependencia = "Dependencias";
        #region Main Methods

        /// <summary>
        /// Inclusão de Dependencia
        /// </summary>
        /// <param name="command">Objeto para inclusão de Dependecia</param>
        /// <returns>Id de Dependecia inserido</returns>
        public Task<long> CreateDependencia(DependenciaModel.CreateUpdateDependenciaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDependencia}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Dependencia
        /// </summary>
        /// <param name="id">Id de alteração de Dependencia</param>
        /// <param name="command">Objeto de alteração de Dependencia</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateDependencia(int id, DependenciaModel.CreateUpdateDependenciaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDependencia}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Dependencia
        /// </summary>
        /// <param name="id">Id de exclusão de Dependecia</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteDependencia(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDependencia}/{id}"));
            return Delete<bool>(requestUrl);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Dependendecia
        /// </summary>
        /// <param name="id">Id de Dependencia ser buscado</param>
        /// <returns>>Retorna o objeto de Dependencia</returns>
        public DependenciaDto GetDependenciaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDependencia}/{id}"));
            return Get<DependenciaDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Dependencia cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Dependencia</returns>
        public List<DependenciaDto> GetDependenciasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDependencia}"));
            return Get<List<DependenciaDto>>(requestUrl);
        }

        #endregion
    }
}