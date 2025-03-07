using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Controle de Presença Client
    /// </summary>
    public partial class DnaApiClient
    {
	    private const string ResourceControlesPresencas = "ControlesPresencas";

        #region Main Methods

        /// <summary>
        /// Inclusão do Controle de Presença
        /// </summary>
        /// <param name="command">Objeto de inclusão do Controle de Presença</param>
        /// <returns>Id do Controle de Presença inserido</returns>
        public Task<long> CreateControlePresenca(ControlePresencaModel.CreateUpdateControlePresencaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControlesPresencas}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração do Controle de Presença
        /// </summary>
        /// <param name="id">Id de alteração do Controle de Presença</param>
        /// <param name="command">Objeto de alteração do Controle de Presença</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateControlePresenca(int id, ControlePresencaModel.CreateUpdateControlePresencaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControlesPresencas}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão do Controle de Presença
        /// </summary>
        /// <param name="id">Id de exclusão do Controle de Presença</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteControlePresenca(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControlesPresencas}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca o Controle de Presença
        /// </summary>
        /// <param name="id">Id de Controle de presença ser buscado</param>
        /// <returns>Retorna o objeto de Controle de Presença</returns>
        public ControlePresencaDto GetControlePresencaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControlesPresencas}/{id}"));
            return Get<ControlePresencaDto>(requestUrl);
        }

        /// <summary>
        /// Busca o Controle de Presença por Evento id
        /// </summary>
        /// <param name="alunoId">Id do Aluno</param>
        /// <returns>Retorna a lista de Controle de Presença</returns>
        public List<ControlePresencaDto> GetControlePresencaByAlunoId(int alunoId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControlesPresencas}/Aluno/{alunoId}"));
            return Get<List<ControlePresencaDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Controles de Presença Cadastrados
        /// </summary>
        /// <param name="pageNumber">Numero da Pagina</param>
        /// <param name="pageSize">Tamnho da Pagina</param>
        /// <returns>Retorna a lista do Controle de Presenças</returns>
        public PaginatedListDto<ControlePresencaDto> GetControlesPresencasAll(int pageNumber = 1, int pageSize = 10)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControlesPresencas}"), $"PageNumber={pageNumber}&PageSize={pageSize}");
            return Get<PaginatedListDto<ControlePresencaDto>>(requestUrl);
        }

        /// <summary>
        /// Busca o Controle de Presença por id Evento
        /// </summary>
        /// <param name="eventoId">Id Evento</param>
        /// <returns>Retorna a lista de Controle de Presença</returns>
        public List<ControlePresencaDto> GetControlesPresencasByEventoId(int eventoId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControlesPresencas}/Evento/{eventoId}"));
            return Get<List<ControlePresencaDto>>(requestUrl);
        }

        /// <summary>
        ///  Busca o Controle de Presença por filtro
        /// </summary>
        /// <param name="searchFilter">Filtro de pesquisa</param>
        /// <returns>Retorna o objeto de controle de presença</returns>
        public Task<ControlesPresencasFilterDto?> GetControlesPresencasByFilter(ControlesPresencasFilterDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControlesPresencas}/Filter"));
            return GetFiltro(requestUrl, searchFilter);
        }

        #endregion

        
    }
}