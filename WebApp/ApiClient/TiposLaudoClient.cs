using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Tipo de Laudo Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceTiposLaudo = "TiposLaudos";
        #region Main Methods

        /// <summary>
        /// Inclusão do Tipo de Laudo
        /// </summary>
        /// <param name="command">Objeto de inclusão do Tipo de Laudo</param>
        /// <returns>Id do Tipo de Laudo inserido</returns>
        public Task<long> CreateTiposLaudo(TiposLaudoModel.CreateUpdateTiposLaudoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTiposLaudo}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração do Tipo de Laudo
        /// </summary>
        /// <param name="id">Id de alteração do Tipo de Laudo</param>
        /// <param name="command">Objeto de alteração do Tipo de Laudo</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateTiposLaudo(int id, TiposLaudoModel.CreateUpdateTiposLaudoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTiposLaudo}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão do Tipo de Laudo
        /// </summary>
        /// <param name="id">Id de exclusão do Tipo de Laudo</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteTiposLaudo(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTiposLaudo}/{id}"));
            return Delete<bool>(requestUrl);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Tipo de Laudo
        /// </summary>
        /// <param name="id">Id do Tipo de Laudo a ser buscado</param>
        /// <returns>Retorna o objeto do Tipo de Laudo</returns>
        public TiposLaudoDto GetTiposLaudoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTiposLaudo}/{id}"));
            return Get<TiposLaudoDto>(requestUrl);
        }

        /// <summary>
        /// busca todos os Tipo de Laudo cadastrados
        /// </summary>
        /// <returns>Retorna a lista do Tipo de Laudo</returns>
        public List<TiposLaudoDto> GetTiposLaudoAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTiposLaudo}"));
            return Get<List<TiposLaudoDto>>(requestUrl);
        }

        #endregion
    }
}