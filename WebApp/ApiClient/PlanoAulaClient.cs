using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        /// <summary>
        /// Plano Aula Client
        /// </summary>

        private const string ResourcePlanoAula = "PlanosAulas";

        #region Main Methods

        /// <summary>
        /// Inclusão de Plano de Aula
        /// </summary>
        /// <param name="command">Objeto de inclusão de Plano Aula</param>
        /// <returns>Id Plano de Aula inserido</returns>
        public Task<long> CreatePlanoAula(PlanoAulaModel.CreateUpdatePlanoAulaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePlanoAula}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Plano de Aula
        /// </summary>
        /// <param name="id">Id de alteração de Plano Aula</param>
        /// <param name="command">Objeto de alteração de Plano Aula</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdatePlanoAula(int id, PlanoAulaModel.CreateUpdatePlanoAulaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePlanoAula}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Plano de Aula
        /// </summary>
        /// <param name="id">Id de exclusão de Plano Aula</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeletePlanoAula(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePlanoAula}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Plano de Aula
        /// </summary>
        /// <param name="id">Id de Plano de Aula a ser buscado</param>
        /// <returns>Retorna o objeto de Plano de Aula</returns>
        public PlanoAulaDto GetPlanoAulaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePlanoAula}/{id}"));
            return Get<PlanoAulaDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Planos de Aulas cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Planos Aulas</returns>
        public List<PlanoAulaDto> GetPlanosAulasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourcePlanoAula}"));
            return Get<List<PlanoAulaDto>>(requestUrl);
        }

        #endregion

    }
}