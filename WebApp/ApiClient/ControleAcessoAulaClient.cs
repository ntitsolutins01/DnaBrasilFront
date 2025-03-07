using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// ControleAcessoAula Client
    /// </summary>
    public partial class DnaApiClient
    {
	    private const string ResourceControleAcessoAula = "ControlesAcessosAulas";

        #region Main Methods

        /// <summary>
        /// Inclusão do Controle de Acesso de Aula
        /// </summary>
        /// <param name="command">Objeto para inclusão do Controle de Acesso de Aula</param>
        /// <returns>Id do Controle de Acesso de Aula inserido</returns>
        public Task<long> CreateControleAcessoAula (ControleAcessoAulaModel.CreateUpdateControleAcessoAulaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleAcessoAula }"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração do Controle de Acesso de Aula
        /// </summary>
        /// <param name="id">Id de alteração de ControleAcessoAula</param>
        /// <param name="command">Objeto de alteração de ControleAcessoAula</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateControleAcessoAula (int id, ControleAcessoAulaModel.CreateUpdateControleAcessoAulaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleAcessoAula }/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão do Controle de Acesso de Aula
        /// </summary>
        /// <param name="id">Id de exclusão do Controle de Acesso de Aula</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteControleAcessoAula (int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleAcessoAula }/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único ControleAcessoAula
        /// </summary>
        /// <param name="id">Id de ControleAcessoAula a ser buscado</param>
        /// <returns>Retorna o objeto de ControleAcessoAula</returns>
        public ControleAcessoAulaDto GetControleAcessoAulaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleAcessoAula}/{id}"));
            return Get<ControleAcessoAulaDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os ControleAcessosAulas cadastrados
        /// </summary>
        /// <returns>Retorna a lista de ControlesAcessosAulas</returns>
        public List<ControleAcessoAulaDto> GetControlesAcessosAulasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleAcessoAula}"));
            return Get<List<ControleAcessoAulaDto>>(requestUrl);
        }

        #endregion
    }
}