using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Talento Esportivo Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceTalentoEsportivo = "TalentosEsportivos";

        #region Main Methods

        /// <summary>
        /// Inclusão de Talento Esportivo
        /// </summary>
        /// <param name="command">Objeto de inclusão de Talento Esportivo</param>
        /// <returns>Id de Talento Esportivo inserido</returns>
        public Task<long> CreateTalentoEsportivo(TalentoEsportivoModel.CreateUpdateTalentoEsportivoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTalentoEsportivo}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Talento Esportivo
        /// </summary>
        /// <param name="id">Id de alteração de Talento Esportivo</param>
        /// <param name="command">Objeto de alteração de Talento Esportivo</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateTalentoEsportivo(int id, TalentoEsportivoModel.CreateUpdateTalentoEsportivoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTalentoEsportivo}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Talento Esportivo
        /// </summary>
        /// <param name="id">Id de exclusão de Talento Esportivo</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteTalentoEsportivo(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTalentoEsportivo}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Busca Talento Esportivo de Aluno por Consulta
        /// </summary>
        /// <param name="id">id de aluno por Consulta </param>
        /// <returns>Retorna uma lista de Talento Esportivo</returns>
        public TalentoEsportivoDto GetTalentoEsportivoByAluno(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTalentoEsportivo}/Aluno/{id}"));
            return Get<TalentoEsportivoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Talento Esportivo cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Talento Esportivo</returns>
        public List<TalentoEsportivoDto> GetTalentoEsportivoAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTalentoEsportivo}"));
            return Get<List<TalentoEsportivoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca um único Talento Esportivo
        /// </summary>
        /// <param name="id">Id de Talento Esportivo a ser buscado</param>
        /// <returns>Retorna o objeto de Talento Esportivo</returns>
        public TalentoEsportivoDto GetTalentoEsportivoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTalentoEsportivo}/{id}"));
            return Get<TalentoEsportivoDto>(requestUrl);
        }
        #endregion

    }
}