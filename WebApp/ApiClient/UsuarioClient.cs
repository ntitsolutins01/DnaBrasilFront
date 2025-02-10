using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Usuário Client
    /// </summary>
	public partial class DnaApiClient
	{
        private const string ResourceUsuario = "Usuarios";
        private const string ResourceUser = "Users";

        #region Main Methods

        public async Task<UsuarioModel.LoginUsuarioRequest?> LoginUsuario(UsuarioModel.LoginUsuarioRequest request)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceUser}/login"));
			return await PostWithResponseBody(requestUrl, request);
		}

        /// <summary>
        /// Inclusão de Usuário
        /// </summary>
        /// <param name="command">Objeto de inclusão de Usuário</param>
        /// <returns>Id do Usuário inserido</returns>
        public async Task<long> CreateUsuario(UsuarioModel.CreateUpdateUsuarioCommand command)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceUsuario}"));
			return await Post(requestUrl, command);
		}

        /// <summary>
        ///  Alteração de Usuário
        /// </summary>
        /// <param name="id">Id de alteração de Usuário</param>
        /// <param name="command">Objeto de alteração de Usuário</param>
        /// <returns>Retorna true ou false</returns>
        public async Task<bool> UpdateUsuario(int id, UsuarioModel.CreateUpdateUsuarioCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceUsuario}/{id}"));
            return await Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Usuário
        /// </summary>
        /// <param name="id">Id de Exclusão de Usuário</param>
        /// <returns>Retorna true ou false</returns>
        public async Task<bool> DeleteUsuario(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceUsuario}/{id}"));
            return await Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Busca todos os Usuário cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Usuário</returns>
        public List<UsuarioDto> GetUsuarioAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceUsuario}"));
            return Get<List<UsuarioDto>>(requestUrl);
        }

        /// <summary>
        ///  Busca um único Usuário
        /// </summary>
        /// <param name="id">Id de Usuário a ser buscado</param>
        /// <returns>Retorna o objeto do Usuário</returns>
        public async Task<UsuarioDto> GetUsuarioById(string id)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceUsuario}/{id}"));
			return Get<UsuarioDto>(requestUrl);
		}

        /// <summary>
        /// Busca Usuário por Id de Rede 
        /// </summary>
        /// <param name="aspNetUserId">id de Usuário por rede </param>
        /// <returns>retorna um objeto de Usuário</returns>
        public async Task<UsuarioDto> GetUsuarioByAspNetUserId(string aspNetUserId)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceUsuario}/AspNetUser/{aspNetUserId}"));
			return Get<UsuarioDto>(requestUrl);
		}

        /// <summary>
        /// Busca Usuário por Cpf
        /// </summary>
        /// <param name="cpf">cpf</param>
        /// <returns>retona uma Lista por Cpf</returns>
        public async Task<UsuarioDto> GetUsuarioByCpf(string cpf)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceUsuario}/Cpf/{cpf}"));
            return Get<UsuarioDto>(requestUrl);
        }

        /// <summary>
        /// Busca Usuário por Email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>Retorna a lista por email </returns>
        public async Task<UsuarioDto> GetUsuarioByEmail(string email)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceUsuario}/Email/{email}"));
            return Get<UsuarioDto>(requestUrl);
        }

        #endregion

    }
}