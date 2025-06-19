using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Evento Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceDocumentoAluno = "DocumentosAluno";

        #region Main Methods

        /// <summary>
        /// Inclusão de Foto e Evento 
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>Id de Foto e Evento inserido</returns>
        public Task<long> CreateDocumentoAluno(List<CreateDocumentoAlunoDto> list)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDocumentoAluno}"));
            return Post(requestUrl, list);
        }

        #endregion

        #region Methods


        /// <summary>
        /// Busca todos os documentos pelo id do aluno
        /// </summary>
        /// <param name="alunoId">Id do aluno</param>
        /// <returns>>Retorna a lista de documentos do aluno</returns>
        public List<DocumentoAlunoDto> GetDocumentosAllByAlunoId(int alunoId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDocumentoAluno}/Aluno/{alunoId}"));
            return Get<List<DocumentoAlunoDto>>(requestUrl);
        }
        #endregion

    }
}