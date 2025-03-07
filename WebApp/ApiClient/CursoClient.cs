using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Curso Client
    /// </summary>
    public partial class DnaApiClient
    {
	    private const string ResourceCurso = "Cursos";

        #region Main Methods
        /// <summary>
        /// Inclusão de Curso
        /// </summary>
        /// <param name="command">Objeto para inclusão do Curso</param>
        /// <returns>Id do Curso inserido</returns>
        public Task<long> CreateCurso (CursoModel.CreateUpdateCursoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCurso }"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Curso
        /// </summary>
        /// <param name="id">Id de alteração de Curso</param>
        /// <param name="command">Objeto de alteração de Curso</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateCurso (int id, CursoModel.CreateUpdateCursoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCurso }/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Curso
        /// </summary>
        /// <param name="id">Id de exclusão de Curso</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteCurso (int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCurso }/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Curso
        /// </summary>
        /// <param name="id">Id do Curso a ser buscado</param>
        /// <returns>Retorna o objeto de Curso</returns>
        public CursoDto GetCursoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCurso}/{id}"));
            return Get<CursoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Cursos cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Cursos</returns>
        public List<CursoDto> GetCursosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCurso}"));
            return Get<List<CursoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Cursos por id 
        /// </summary>
        /// <param name="tipoCursoId">Id do tipo de curso</param>
        /// <returns>Retorna a lista por Curso id</returns>
        public List<CursoDto> GetCursosAllByTipoCursoId(int tipoCursoId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCurso}/TipoCurso/{tipoCursoId}"));
            return Get<List<CursoDto>>(requestUrl);
        }

        #endregion
    }
}