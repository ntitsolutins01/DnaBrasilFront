using System.Collections;
using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        /// <summary>
        /// profissional Client
        /// </summary>
        private const string ResourceProfissional = "Profissionais";

        #region Main Methods

        /// <summary>
        /// Inclusão de Profissionais
        /// </summary>
        /// <param name="command">Objeto de inclusão de Profissionais</param>
        /// <returns>Id de Profissional inserido</returns>
        public Task<long> CreateProfissional(ProfissionalModel.CreateUpdateProfissionalCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceProfissional}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Profissionais
        /// </summary>
        /// <param name="id">Id de alteração de Profissionais</param>
        /// <param name="command">Objeto de alteração de Profissionais</param>
        /// <returns>Retorna true ou false</returns>
		public Task<bool> UpdateProfissional(int id, ProfissionalModel.CreateUpdateProfissionalCommand command)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceProfissional}/{id}"));
			return Put(requestUrl, command);
		}

        /// <summary>
        /// exclusão de Profissionais
        /// </summary>
        /// <param name="id">Id de exclusão de Profissionais</param>
        /// <returns>Retorna true ou false</returns>
		public Task<bool> DeleteProfissional(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceProfissional}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Profissional
        /// </summary>
        /// <param name="id">Id do profissional a ser buscado</param>
        /// <returns>Retorna o objeto da Profissional</returns>
        public ProfissionalDto GetProfissionalById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceProfissional}/{id}"));
            return Get<ProfissionalDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Profissionaiss cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Profissionais</returns>
        public List<ProfissionalDto> GetProfissionalAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceProfissional}"));
            return Get<List<ProfissionalDto>>(requestUrl);
        }

        /// <summary>
        /// Busca profissional por Cpf
        /// </summary>
        /// <param name="cpf">cpf</param>
        /// <returns>retorna Profissional por cpf</returns>
        public ProfissionalDto GetProfissionalByCpf(string cpf)
        {
	        var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
		        $"{ResourceProfissional}/Cpf/{cpf}"));
	        return Get<ProfissionalDto>(requestUrl);
        }

        /// <summary>
        ///  Busca o Profissional por Email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>retorna um Profissional por Email</returns>
        public async Task<ProfissionalDto> GetProfissionalByEmail(string email)
        {
	        var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
		        $"{ResourceProfissional}/Email/{email}"));
	        return Get<ProfissionalDto>(requestUrl);
        }

        /// <summary>
        /// busca uma lista de Profissionais por Localidade
        /// </summary>
        /// <param name="id">id de Profissionais por  localidade</param>
        /// <returns>retrona uma lista de Profissionais</returns>
        public List<ProfissionalDto> GetProfissionaisByLocalidade(int id)
        {
	        var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
		        $"{ResourceProfissional}/Localidade/{id}"));
	        return Get<List<ProfissionalDto>>(requestUrl);
        }

		#endregion

    }
}