using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Evento Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceEvento = "Eventos";
        private const string ResourceFotoEvento = "FotosEvento";

        #region Main Methods

        /// <summary>
        /// Inclusão de Evento
        /// </summary>
        /// <param name="command">Objeto para inclusão de Evento</param>
        /// <returns>Id de Evento inserido</returns>
        public Task<long> CreateEvento(EventoModel.CreateUpdateEventoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEvento}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Evento
        /// </summary>
        /// <param name="id">Id de alteração de Evento</param>
        /// <param name="command">Objeto de alteração da Evento</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateEvento(int id, EventoModel.CreateUpdateEventoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEvento}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Evento
        /// </summary>
        /// <param name="id">Id de exclusão de Evento</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteEvento(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEvento}/{id}"));
            return Delete<bool>(requestUrl);
        }

        /// <summary>
        /// Inclusão de Foto e Evento 
        /// </summary>
        /// <param name="list">list</param>
        /// <returns>Id de Foto e Evento inserido</returns>
        public Task<long> CreateFotoEvento(List<CreateFotoEventoDto> list)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFotoEvento}"));
            return Post(requestUrl, list);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Evento
        /// </summary>
        /// <param name="id">Id de Evento a ser buscado</param>
        /// <returns>Retorna o objeto de Evento</returns>
        public EventoDto GetEventoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEvento}/{id}"));
            return Get<EventoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos as Fotos por Evento Id
        /// </summary>
        /// <param name="eventoId">Evento por id </param>
        /// <returns>>Retorna o objeto de Fotos por Evento Id</returns>
        public List<FotoEventoDto> GetFotosAllByEventoId(int eventoId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFotoEvento}/Fotos/{eventoId}"));
            return Get<List<FotoEventoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Eventos cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Eventos</returns>
        public async Task<List<EventoDto>> GetEventosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEvento}"));
            return Get<List<EventoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca Eventos por Filtro
        /// </summary>
        /// <param name="searchFilter">Filtro para pesquisa de Evento</param>
        /// <returns>Retorna a lista de Evento por Filtro</returns>
        public Task<EventosFilterDto?> GetEventosByFilter(EventosFilterDto searchFilter)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEvento}/Filter"));
            return GetFiltro(requestUrl, searchFilter);
        }
        #endregion


    }
}