using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Questionario Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceQuestionario = "Questionarios";
        #region Main Methods

        /// <summary>
        /// Inclusão de Questionário
        /// </summary>
        /// <param name="command">Objeto de inclusão de Questionário</param>
        /// <returns>Id do Questionario inserido</returns>
        public Task<long> CreateQuestionario(QuestionarioModel.CreateUpdateQuestionarioCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestionario}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Questionário
        /// </summary>
        /// <param name="id">Id de alteração de Questionário</param>
        /// <param name="command">Objeto de alteração de Questionário</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateQuestionario(int id, QuestionarioModel.CreateUpdateQuestionarioCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestionario}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Questionário
        /// </summary>
        /// <param name="id">Id de exclusao de Questionário</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteQuestionario(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestionario}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Questionário
        /// </summary>
        /// <param name="id">Id da Questionário a ser buscado</param>
        /// <returns>Retorna o objeto do Quetionário</returns>
        public QuestionarioDto GetQuestionarioById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestionario}/{id}"));
            return Get<QuestionarioDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Questionários cadastrados
        /// </summary>
        /// <returns>>Retorna a Lista de Questionário</returns>
        public List<QuestionarioDto> GetQuestionarioAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestionario}"));
            return Get<List<QuestionarioDto>>(requestUrl);
        }

        /// <summary>
        ///  Busca Quetionário por Tipo de Laudo
        /// </summary>
        /// <param name="id">id de Questionário por Laudo</param>
        /// <returns>Retorna a uma Lista de Questionário</returns>
        public List<QuestionarioDto> GetQuestionarioByTipoLaudo(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceQuestionario}/TipoLaudo/{id}"));
            return Get<List<QuestionarioDto>>(requestUrl);
        }

        #endregion
    }
}