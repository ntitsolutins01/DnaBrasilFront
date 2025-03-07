using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Atividade Client
    /// </summary>
    public partial class DnaApiClient
    {
	    private const string ResourceAtividade = "Atividades";

        #region Main Methods

        /// <summary>
        /// Inclusão de Atividade
        /// </summary>
        /// <param name="command">Objeto para a inclusão de Atividade</param>
        /// <returns>ID da Atividade inserido</returns>
        public Task<long> CreateAtividade (AtividadeModel.CreateUpdateAtividadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAtividade }"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Atividade
        /// </summary>
        /// <param name="id">ID da alteração de Atividade</param>
        /// <param name="command">Objeto para alteração de Atividade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateAtividade (int id, AtividadeModel.CreateUpdateAtividadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAtividade }/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        ///  Exclusão de Atividade
        /// </summary>
        /// <param name="id">Id da exclusão de atividade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteAtividade (int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAtividade }/{id}"));
            return Delete<bool>(requestUrl);
        }

        /// <summary>
        /// Inclusão de Atividade e Alunos
        /// </summary>
        /// <param name="command">Objeto para inclusão de Atividade de Alunos</param>
        /// <returns>Retorna o Id da Atividade</returns>
        public Task<long> CreateAtividadeAluno(AtividadeModel.CreateUpdateAtividadeAlunosCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAtividade}/Alunos"));
            return Post(requestUrl, command);
        }

        /// <summary>
        ///Alteração de Atividade de Alunos
        /// </summary>
        /// <param name="id">Id da alteração de atividade</param>
        /// <param name="command">Objeto para alteração de Atividade de alunos</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateAtividadeAluno(int id, AtividadeModel.CreateUpdateAtividadeAlunosCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAtividade}/{id}/Alunos"));
            return Put(requestUrl, command);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca por uma única Atividade
        /// </summary>
        /// <param name="id">Id da atividade a ser buscado</param>
        /// <returns>Retorna o objeto da atividade</returns>
        public AtividadeDto GetAtividadeById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAtividade}/{id}"));
            return Get<AtividadeDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Atividades Cadastradas
        /// </summary>
        /// <returns>Retorna a lista de atividades</returns>
        public List<AtividadeDto> GetAtividadesAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAtividade}"));
            return Get<List<AtividadeDto>>(requestUrl);
        }

        /// <summary>
        /// Busca a lista de turmas pelo Id da modalidade e ID do profissional
        /// </summary>
        /// <param name="modalidadeId">ID da modalidade</param>
        /// <param name="profissionalId">ID do profissional</param>
        /// <returns>Retorna a lista de turmas</returns>
        public List<AtividadeDto> GetTurmasByModalidadeIdProfissionalId(int modalidadeId, int profissionalId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAtividade}/Modalidade/{modalidadeId}/Profissional/{profissionalId}"));
            return Get<List<AtividadeDto>>(requestUrl);
        }

        /// <summary>
        /// Busca o alunos pelo ID da atividade
        /// </summary>
        /// <param name="id">ID da atividade</param>
        /// <returns>Retorna a lista de alunos</returns>
        public List<AtividadeAlunosDto> GetAtividadeAlunosByAtividadeId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAtividade}/{id}/Alunos"));
            return Get<List<AtividadeAlunosDto>>(requestUrl);
        }

        /// <summary>
        /// Busca a atividades pelo ID da localidade
        /// </summary>
        /// <param name="id">Id da localidade</param>
        /// <returns>Retorna a lista de atividades</returns>
        public async Task<List<AtividadeDto>> GetAtividadeByLocalidadeId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAtividade}/Localidade/{id}"));
            return Get<List<AtividadeDto>>(requestUrl);
        }

        #endregion


    }
}