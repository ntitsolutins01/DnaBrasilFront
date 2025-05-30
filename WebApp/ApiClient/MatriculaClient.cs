using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Matricula Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceMatricula = "Matriculas";
        #region Main Methods

        /// <summary>
        /// Inclusão da Matricula
        /// </summary>
        /// <param name="command">Objeto de inclusão da Matricula</param>
        /// <returns>Id da Matricula inserido</returns>
        public Task<long> CreateMatricula(MatriculaModel.CreateUpdateMatriculaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMatricula}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração da Matricula
        /// </summary>
        /// <param name="id">Id de alteração da Matricula</param>
        /// <param name="command">Objeto de alteração da Matricula</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateMatricula(int id, MatriculaModel.CreateUpdateMatriculaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMatricula}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão da Matricula
        /// </summary>
        /// <param name="id">Id de exclusão da Matricula</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteMatricula(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMatricula}/{id}"));
            return Delete<bool>(requestUrl);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Matricula
        /// </summary>
        /// <param name="id">Id de Matricula a ser buscado</param>
        /// <returns>Retorna o objeto de Matricula</returns>
        public MatriculaDto GetMatriculaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMatricula}/{id}"));
            return Get<MatriculaDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Matriculas cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Matricula</returns>
        public List<MatriculaDto> GetMatriculaAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceMatricula}"));
            return Get<List<MatriculaDto>>(requestUrl);
        }

        #endregion
    }
}