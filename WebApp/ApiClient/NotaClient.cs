using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Nota Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceNota = "Notas";

        #region Main Methods

        /// <summary>
        /// Inclusão de Nota
        /// </summary>
        /// <param name="command">Objeto de inclusão da Nota</param>
        /// <returns>Id de Nota inserido</returns>
        public Task<long> CreateNota(NotaModel.CreateUpdateNotaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceNota}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Nota
        /// </summary>
        /// <param name="id">Id de alteração da Nota</param>
        /// <param name="command">Objeto de alteração da Nota</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateNota(int id, NotaModel.CreateUpdateNotaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceNota}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Nota
        /// </summary>
        /// <param name="id">Id de exclusao da Nota</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteNota(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceNota}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Nota
        /// </summary>
        /// <param name="id">Id da Nota a ser buscada</param>
        /// <returns>Retorna o objeto da Nota</returns>
        public NotaDto GetNotaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceNota}/{id}"));
            return Get<NotaDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Notas cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Nota</returns>
        public List<NotaDto> GetNotasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceNota}"));
            return Get<List<NotaDto>>(requestUrl);
        }

        //public NotaDto GetNotaByAlunoIdDisciplinaId(int alunoId, int disciplinaId)
        //{
        //	var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
        //		$"{ResourceNota}/Aluno/{alunoId}/Disciplina/{disciplinaId}"));
        //	return Get<NotaDto>(requestUrl);
        //}

        #endregion


    }
}