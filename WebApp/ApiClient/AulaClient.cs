using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Aula Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceAula = "Aulas";

        #region Main Methods

        /// <summary>
        /// Inclusão de Aula
        /// </summary>
        /// <param name="command">Objeto para inclusão de Aula</param>
        /// <returns>Id de Aula inserido</returns>
        public Task<long> CreateAula(AulaModel.CreateUpdateAulaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Aula
        /// </summary>
        /// <param name="id">Id de alteração de Aula</param>
        /// <param name="command">Objeto de alteração de Aula</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateAula(int id, AulaModel.CreateUpdateAulaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Aula
        /// </summary>
        /// <param name="id">Id de exclusão de Aula</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteAula(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Aula
        /// </summary>
        /// <param name="id">Id da Aula a ser buscada</param>
        /// <returns>Retorna o objeto de Aula</returns>
        public AulaDto GetAulaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula}/{id}"));
            return Get<AulaDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Aulas cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Aula</returns>
        public List<AulaDto> GetAulasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula}"));
            return Get<List<AulaDto>>(requestUrl);
        }
        public List<AulaDto> GetAulasAllByModuloEadId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula}/ModuloEad/{id}"));
            return Get<List<AulaDto>>(requestUrl);
        }
        public List<AulaDto> GetAulasByCursoId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula}/Curso/{id}"));
            return Get<List<AulaDto>>(requestUrl);
        }

        #endregion
    }
}