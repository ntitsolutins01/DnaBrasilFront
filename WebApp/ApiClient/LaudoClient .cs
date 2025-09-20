using Newtonsoft.Json;
using WebApp.Dto;
using WebApp.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        /// Alteração de Educacional de um Laudo
        /// </summary>
        /// <param name="id">Id de alteração de Laudo</param>
        /// <param name="command">Objeto de alteração de Laudo</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateLaudoEducacional(int id, LaudoModel.UpdateLaudoEducacionalCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/{id}/Educacional"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusao de laudo
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
        /// busca todos os Laudo por Aluno
        /// </summary>
        /// <param name="id">id que busca laudo por Aluno</param>
        /// <returns>retorna a lista de Laudo por Aluno</returns>
        public LaudoDto? GetLaudoByAluno(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Aluno/{id}"));
            return Get<LaudoDto?>(requestUrl);
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
        /// <param name="id">id que busca Ecaminhamento de Saúde por id</param>
        /// <returns>retorna a lista de Encaminhamento de Saúde por id</returns>
        public EncaminhamentoDto GetEncaminhamentoBySaudeId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Encaminhamento/Saude/{id}"));
            return Get<EncaminhamentoDto>(requestUrl);
        }

        /// <summary>
        /// Busca Encaminhamento de Qualidade de Vida por id
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
        /// <returns>retorna a lista de Encaminhamento Vocacional</returns>
        public List<EncaminhamentoDto> GetEncaminhamentoByVocacional()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Encaminhamentos/Vocacional"));
            return Get<List<EncaminhamentoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca Encaminhamento de Consumo Alimentar por id
        /// </summary>
        /// <param name="id">Id que busca Encaminhamento de Consumo Alimentar por id</param>
        /// <returns>retorna a lista de Encaminhamento de Consumo Alimentar por id</returns>
        public EncaminhamentoDto GetEncaminhamentoByConsumoAlimentarId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Encaminhamento/ConsumoAlimentar/{id}"));
            return Get<EncaminhamentoDto>(requestUrl);
        }

        /// <summary>
        /// Busca Encaminhamento de Saude Bucal por id
        /// </summary>
        /// <param name="id">Id que busca Encaminhamento de Saude Bucal por id</param>
        /// <returns>retorna a lista de Encaminhamento de Saude Bucal por id</returns>
        public EncaminhamentoDto GetEncaminhamentoBySaudeBucalId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Encaminhamento/SaudeBucal/{id}"));
            return Get<EncaminhamentoDto>(requestUrl);
        }

        /// <summary>
        /// Busca Desempenho por Aluno
        /// </summary>
        /// <param name="id">id que busca Desempenho por Aluno</param>
        /// <returns>retorna lista de Desempenho por Aluno</returns>
        public DesempenhoDto GetDesempenhoByAluno(int? id)
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

        /// <summary>
        /// Busca Laudos Resumidos por Filtro
        /// </summary>
        /// <param name="searchFilter">filtro para pesquisa de Laudos Resumidos</param>
        /// <returns>retorna a lista de Laudos Resumidos por Filtro</returns>
        public Task<LaudosResumidosFilterDto?> GetLaudosResumidosByFilter(LaudosResumidosFilterDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/ResumidosFilter"));
            return GetFiltro(requestUrl, searchFilter);
        }

        /// <summary>
        /// Processa o gabarito realizado pelo aluno
        /// </summary>
        /// <param name="imagemBytes">Imagem do gabarito</param>
        /// <returns>Retorna respostas do gabarito</returns>
        public async Task<Dictionary<string, object>> ProcessarGabarito(byte[] imagemBytes)
        {
            var requestUrl = CreateRequestUri($"{ResourceLaudo}/ProcessarGabarito");

            return await PostDict(requestUrl, imagemBytes);
        }

        /// <summary>
        /// Inclusão de Educacional
        /// </summary>
        /// <param name="command">Objeto de inclusão de Educacional</param>
        /// <returns>Id de Educacional inserido</returns>
        public Task<long> CreateEducacional(LaudoModel.CreateUpdateEducacionalCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"Educacionais"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Busca Alternativas de id do Educacional
        /// </summary>
        /// <param name="id">Id que busca Alternativas de Educacional por id</param>
        /// <returns>retorna a lista de Alternativas de Educacional por id</returns>
        public AlternativasDto GetAlternativasByEducacionalId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Alternativas/Educacional/{id}"));
            return Get<AlternativasDto>(requestUrl);
        }

        /// <summary>
        /// Busca Educacionais de pelo id do Aluno
        /// </summary>
        /// <param name="id">Id que busca Educacionais de Aluno por id</param>
        /// <returns>retorna a lista de Educacionais de Aluno por id</returns>
        public List<EducacionalDto> GetEducacionaisByAluno(int? id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceLaudo}/Educacional/Aluno/{id}"));
            return Get<List<EducacionalDto>>(requestUrl);
        }
    }

    #endregion

}
