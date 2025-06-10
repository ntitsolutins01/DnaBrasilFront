using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Perfil client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourcePerfil = "Perfis";

        #region Main Methods

        /// <summary>
        /// inclusão de Perfis
        /// </summary>
        /// <param name="command">Objeto de inclusão de Perfis</param>
        /// <returns>Id de Perfil inserido</returns>
        public Task<long> CreatePerfil(PerfilModel.CreateUpdateCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePerfil}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// alteração de Perfis
        /// </summary>
        /// <param name="id">Id de alteração de Perfis</param>
        /// <param name="command">Objeto de alteração de Perfis</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdatePerfil(int id, PerfilModel.CreateUpdateCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePerfil}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Perfis
        /// </summary>
        /// <param name="id">Id de exclusao de Perfis</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeletePerfil(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePerfil}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca todas as Perfis cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Perfis</returns>
        public List<PerfilDto> GetPerfilAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePerfil}"));
            return Get<List<PerfilDto>>(requestUrl);
        }

        /// <summary>
        ///  busca um único Perfil
        /// </summary>
        /// <param name="id">Id de Perfis a ser buscada</param>
        /// <returns>Retorna o objeto de Perfis</returns>
        public PerfilDto GetPerfilById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePerfil}/{id}"));
            return Get<PerfilDto>(requestUrl);
        }

        /// <summary>
        /// Busca Perfil por id de Funçao de Rede Asp
        /// </summary>
        /// <param name="aspNetRoleId">id de Funçao de rede Asp</param>
        /// <returns>retorna a lista de Perfis</returns>
        public PerfilDto GetPerfilByAspNetRoleId(string aspNetRoleId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePerfil}/AspNetRoleId/{aspNetRoleId}"));
            return Get<PerfilDto>(requestUrl);
        }

        #endregion
    }
}