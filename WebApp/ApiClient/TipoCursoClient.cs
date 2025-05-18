using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Tipo de Curso Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceTipoCurso = "TiposCursos";

        #region Main Methods

        /// <summary>
        /// Inclusão de Tipo de Curso
        /// </summary>
        /// <param name="command">>Objeto de inclusão de Tipo de Curso</param>
        /// <returns>Retorna o objeto de Tipoo de curso</returns>
        public Task<long> CreateTipoCurso(TipoCursoModel.CreateUpdateTipoCursoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoCurso}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        ///  Alteração de Tipo de Curso
        /// </summary>
        /// <param name="id">Id de alteração de Tipo de Curso</param>
        /// <param name="command">Objeto de alteração de Tipo de Curso</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateTipoCurso(int id, TipoCursoModel.CreateUpdateTipoCursoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoCurso}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Tipo de Curso
        /// </summary>
        /// <param name="id">Id de exclusão de Tipo de Curso</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteTipoCurso(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoCurso}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Busca um único Tipo de Curso
        /// </summary>
        /// <param name="id">Id de Tipo de Curso a ser buscado</param>
        /// <returns>Retorna o objeto da Aula</returns>
        public TiposCursoDto GetTipoCursoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoCurso}/{id}"));
            return Get<TiposCursoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Tipos de Cursos cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Tipo de Curso</returns>
        public List<TiposCursoDto> GetTipoCursosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoCurso}"));
            return Get<List<TiposCursoDto>>(requestUrl);
        }

        #endregion
    }
}