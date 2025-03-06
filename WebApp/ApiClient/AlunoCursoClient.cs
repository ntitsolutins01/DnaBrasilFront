using System.Collections;
using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// AlunoCurso Client
    /// </summary>
    public partial class DnaApiClient
    {
	    private const string ResourceAlunoCurso = "AlunosCursos";

        #region Main Methods

        /// <summary>
        /// Alteração de AlunoCurso
        /// </summary>
        /// <param name="id">Id de alteração de AlunoCurso</param>
        /// <param name="command">Objeto de alteração de AlunoCurso</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateAlunoCurso (int id, AlunoCursoCertificadoModel.CreateUpdateAlunoCursoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunoCurso }/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de AlunoCurso
        /// </summary>
        /// <param name="id">Id de exclusão de AlunoCurso</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteAlunoCurso (int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunoCurso }/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única AlunoCurso
        /// </summary>
        /// <param name="id">Id da AlunoCurso a ser buscada</param>
        /// <returns>Retorna o objeto de AlunoCurso</returns>
        public AlunoCursoDto GetAlunoCursoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunoCurso}/{id}"));
            return Get<AlunoCursoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as AlunosCursos cadastradas
        /// </summary>
        /// <returns>Retorna a lista de AlunoCurso</returns>
        public List<AlunoCursoDto> GetAlunosCursosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunoCurso}"));
            return Get<List<AlunoCursoDto>>(requestUrl);
        }

        #endregion
    }
}