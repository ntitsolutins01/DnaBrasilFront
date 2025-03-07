using System.Collections;
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
        /// Inclusão de aula
        /// </summary>
        /// <param name="command">Objeto para a inclusão de aula</param>
        /// <returns>ID de aula inserido</returns>
        public Task<long> CreateAula (AulaModel.CreateUpdateAulaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula }"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Aula
        /// </summary>
        /// <param name="id">ID de alteração de aula</param>
        /// <param name="command">Objeto de alteração de aula</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateAula (int id, AulaModel.CreateUpdateAulaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula }/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de aula
        /// </summary>
        /// <param name="id">ID de exclusão de aula</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteAula (int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula }/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca de uma única aula
        /// </summary>
        /// <param name="id">ID da aula a ser buscada</param>
        /// <returns>Retorna o objeto de aula</returns>
        public AulaDto GetAulaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula}/{id}"));
            return Get<AulaDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as aulas cadastradas
        /// </summary>
        /// <returns>Retorna a lista de aula</returns>
        public List<AulaDto> GetAulasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula}"));
            return Get<List<AulaDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todas as aulas por ID de módulo EAD
        /// </summary>
        /// <param name="id">ID do módulo EAD a ser buscado</param>
        /// <returns>Retorna o módulo EAD</returns>
        public List<AulaDto> GetAulasAllByModuloEadId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAula}/ModuloEad/{id}"));
            return Get<List<AulaDto>>(requestUrl);
        }

        #endregion
    }
}