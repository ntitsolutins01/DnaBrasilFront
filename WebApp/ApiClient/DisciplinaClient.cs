using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Disciplina Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceDisciplina = "Disciplinas";
        #region Main Methods

        /// <summary>
        /// Inclusão de Disciplina
        /// </summary>
        /// <param name="command">Objeto de inclusão da Disciplina</param>
        /// <returns>Id de Disciplina inserido</returns>
        public Task<long> CreateDisciplina(DisciplinaModel.CreateUpdateDisciplinaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDisciplina}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        ///  Alteração de Disciplina
        /// </summary>
        /// <param name="id">Id de alteração da Disciplina</param>
        /// <param name="command">Objeto de alteração da Disciplina</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateDisciplina(int id, DisciplinaModel.CreateUpdateDisciplinaCommand command)
        {
	        var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
		        $"{ResourceDisciplina}/{id}"));
	        return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Disciplina
        /// </summary>
        /// <param name="id">Id de exclusão da Disciplina</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteDisciplina(int id)
        {
	        var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
		        $"{ResourceDisciplina}/{id}"));
	        return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca todas as Disciplinas Cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Disciplinas</returns>
        public List<DisciplinaDto> GetDisciplinasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDisciplina}"));
            return Get<List<DisciplinaDto>>(requestUrl);
        }

        /// <summary>
        ///  busca uma única Disciplina
        /// </summary>
        /// <param name="id">Id da Disciplina a ser buscada</param>
        /// <returns>Retorna o objeto da Disciplina</returns>
        public DisciplinaDto GetDisciplinaById(int id)
        {
	        var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
		        $"{ResourceDisciplina}/{id}"));
	        return Get<DisciplinaDto>(requestUrl);
        }
		#endregion
	}
}