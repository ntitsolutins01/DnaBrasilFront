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
        /// Inclusão de Perfil
        /// </summary>
        /// <param name="command">Objeto de inclusão de Perfil</param>
        /// <returns>Id de Perfil inserido</returns>
        public Task<long> CreatePerfil(PerfilModel.CreateUpdateCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePerfil}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Perfil
        /// </summary>
        /// <param name="id">Id de alteração de Perfil</param>
        /// <param name="command">Objeto de alteração de Perfil</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdatePerfil(int id, PerfilModel.CreateUpdateCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePerfil}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Perfil
        /// </summary>
        /// <param name="id">Id de exclusão de Perfil</param>
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
        /// <returns>Retorna a lista de Perfil</returns>
        public List<PerfilDto> GetPerfilAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePerfil}"));
            return Get<List<PerfilDto>>(requestUrl);
        }

        /// <summary>
        ///  Busca um único Perfil
        /// </summary>
        /// <param name="id">Id de Perfil a ser buscada</param>
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
        /// <param name="aspNetRoleId">Id de Função de rede Asp</param>
        /// <returns>Retorna a lista de Perfis</returns>
        public PerfilDto GetPerfilByAspNetRoleId(string aspNetRoleId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePerfil}/AspNetRoleId/{aspNetRoleId}"));
            return Get<PerfilDto>(requestUrl);
		}

		#endregion
	}
}