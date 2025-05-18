using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Texto e Laudo Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceTextoLaudo = "TextosLaudos";

        #region Main Methods

        /// <summary>
        /// Inclusão de Texto e Laudo
        /// </summary>
        /// <param name="command">Objeto de inclusão de Texto e Laudo</param>
        /// <returns>Retorna o objeto de Texto e Laudo</returns>
        public Task<long> CreateTextoLaudo(TextoLaudoModel.CreateUpdateTextoLaudoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTextoLaudo}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Texto e Laudo
        /// </summary>
        /// <param name="id">Id de alteração de Texto e Laudo</param>
        /// <param name="command">Objeto de alteração de Texto e Laudo</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateTextoLaudo(int id, TextoLaudoModel.CreateUpdateTextoLaudoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTextoLaudo}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Texto e Laudo
        /// </summary>
        /// <param name="id">Id de Exclusão de Texto e Laudo</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteTextoLaudo(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTextoLaudo}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Texto e Laudo
        /// </summary>
        /// <param name="id">Id de Texto e Laudo a ser buscado</param>
        /// <returns>Retorna o objeto de Texto e Laudo</returns>
        public TextoLaudoDto GetTextoLaudoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTextoLaudo}/{id}"));
            return Get<TextoLaudoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Texto e Laudos cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Texto e Laudos</returns>
        public List<TextoLaudoDto> GetTextosLaudosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTextoLaudo}"));
            return Get<List<TextoLaudoDto>>(requestUrl);
        }

        #endregion
    }
}