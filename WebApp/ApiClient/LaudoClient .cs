using System.Drawing.Printing;
using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Laudo Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceLaudo = "Laudos";
        #region Main Methods

        /// <summary>
        /// Inclusão de Laudo
        /// </summary>
        /// <param name="command">Objeto de inclusão de Laudo</param>
        /// <returns>Id de Laudo inserido</returns>
        public Task<long> CreateLaudo(LaudoModel.CreateUpdateLaudoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Laudo
        /// </summary>
        /// <param name="id">Id de alteração de Laudo</param>
        /// <param name="command">Objeto de alteração de Laudo</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateLaudo(int id, LaudoModel.CreateUpdateLaudoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de laudo
        /// </summary>
        /// <param name="id">Id de exclusão de Laudo</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteLaudo(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca Laudos por Id
        /// </summary>
        /// <param name="id">Id que busca Laudos por id</param>
        /// <returns>Retorna o objeto de Laudo</returns>
        public LaudoDto GetLaudoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/{id}"));
            return Get<LaudoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Laudo por Aluno
        /// </summary>
        /// <param name="id">Id que busca Laudo por Aluno</param>
        /// <returns>Retorna a lista de Laudo por Aluno</returns>
        public LaudoDto GetLaudoByAluno(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Aluno/{id}"));
            return Get<LaudoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Laudos cadastradas
        /// </summary>
        /// <param name="pageNumber">Número de Página</param>
        /// <param name="pageSize">Tamanho da Página</param>
        /// <returns>Retorna o objeto de Laudo</returns>
        public PaginatedListDto<LaudoDto> GetLaudosAll(int pageNumber = 1, int pageSize = 10)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}"), $"PageNumber={pageNumber}&PageSize={pageSize}");
            return Get<PaginatedListDto<LaudoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca Encaminhamento de Saúde por Id
        /// </summary>
        /// <param name="id">Id que busca Ecaminhamento de Saúde por id</param>
        /// <returns>Retorna a lista de Encaminhamento de Saúde por id</returns>
        public EncaminhamentoDto GetEncaminhamentoBySaudeId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Encaminhamento/Saude/{id}"));
            return Get<EncaminhamentoDto>(requestUrl);
        }

        /// <summary>
        /// Busca Encaminhamento de Quialidade de Vida por id
        /// </summary>
        /// <param name="id">Id que busca Encaminhamento de Qualidade de Vidas por id</param>
        /// <returns>retorna a lista de Encaminhamento de Qualidade de Vida por id</returns>
        public List<EncaminhamentoDto> GetEncaminhamentoByQualidadeDeVidaId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Encaminhamento/QualidadeDeVida/{id}"));
            return Get<List<EncaminhamentoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca Encaminhamento por Vocacional
        /// </summary>
        /// <returns>Retorna a lista de Encaminhamento Vocacional</returns>
        public List<EncaminhamentoDto> GetEncaminhamentoByVocacional()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Encaminhamentos/Vocacional"));
            return Get<List<EncaminhamentoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca Desempenho por Aluno
        /// </summary>
        /// <param name="id">Id que busca Desempenho por Aluno</param>
        /// <returns>Retorna lista de Desempenho por Aluno</returns>
        public DesempenhoDto GetDesempenhoByAluno(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Desempenho/{id}"));
            return Get<DesempenhoDto>(requestUrl);
        }

        /// <summary>
        /// Busca Laudo por Filtro
        /// </summary>
        /// <param name="searchFilter">filtro para pesquisa de Laudo</param>
        /// <returns>retorna a lista de Laudo por Filtro</returns>
        public Task<LaudosFilterDto?> GetLaudosByFilter(LaudosFilterDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Filter"));
            return GetFiltro(requestUrl, searchFilter);
        }

        #endregion

    }
}