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
        private const string ResourceAlunosCursos = "Alunos/AlunosCursos";

        #region Main Methods

        /// <summary>
        /// Alteração de AlunoCurso
        /// </summary>
        /// <param name="id">Id de alteração de AlunoCurso</param>
        /// <param name="command">Objeto de alteração de AlunoCurso</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateAlunoCurso(int alunoId, int cursoId, AlunoCursoCertificadoModel.UpdateProgressoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunosCursos}/{alunoId}/{cursoId}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de AlunoCurso
        /// </summary>
        /// <param name="id">Id de exclusão de AlunoCurso</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteAlunoCurso(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunosCursos}/{id}"));
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
                $"{ResourceAlunosCursos}/{id}"));
            return Get<AlunoCursoDto>(requestUrl);
        }

        /// <summary>
        /// Busca uma lista de AlunosCursos
        /// </summary>
        /// <param name="alunoId">Id da AlunoCurso a ser buscada</param>
        /// <returns>Retorna uma lista de AlunosCursos</returns>
        public List<AlunoCursoDto> GetAlunoCursosByAlunoId(int alunoId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunosCursos}/Aluno/{alunoId}"));
            return Get<List<AlunoCursoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todos os alunos de um curso
        /// </summary>
        /// <param name="cursoId">Id do curso</param>
        /// <returns>Retorna lista dos AlunosCursos</returns>
        public List<AlunoCursoDto> GetAlunosCursosByCursoId(int cursoId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunosCursos}/Curso/{cursoId}"));
            return Get<List<AlunoCursoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todas as AlunosCursos cadastradas
        /// </summary>
        /// <returns>Retorna a lista de AlunoCurso</returns>
        public List<AlunoCursoDto> GetAlunosCursosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunosCursos}"));
            return Get<List<AlunoCursoDto>>(requestUrl);
        }

        #endregion
    }
}