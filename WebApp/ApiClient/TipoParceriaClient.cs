using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Tipo de Parceria
    /// </summary>
    public partial class DnaApiClient
    {
	    private const string ResourceTipoParceria = "TipoParcerias";

        #region Main Methods

        /// <summary>
        /// Inclusão do Tipo de Parceria
        /// </summary>
        /// <param name="command">Objeto para inclusão do Tipo de Parceria</param>
        /// <returns>Id do Tipo de Parceria inserido</returns>
        public Task<long> CreateTipoParceria(TipoParceriaModel.CreateUpdateTipoParceriaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoParceria}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        ///  Alteração do Tipo de Parceria
        /// </summary>
        /// <param name="id">Id de alteração do Tipo de Parceria</param>
        /// <param name="command">Objeto de alteração do Tipo de Parceria</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateTipoParceria(int id, TipoParceriaModel.CreateUpdateTipoParceriaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoParceria}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão do Tipo de Parceria
        /// </summary>
        /// <param name="id">Id de exclusão do Tipo de Parceria</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteTipoParceria(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoParceria}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Busca um único Tipo de Parceria
        /// </summary>
        /// <param name="id">Id do Tipo de Parceria a ser buscado</param>
        /// <returns>Retorna o objeto do Tipo de Parceria</returns>
        public TipoParceriaDto GetTipoParceriaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoParceria}/{id}"));
            return Get<TipoParceriaDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Tipos de Parcerias cadastradas
        /// </summary>
        /// <returns>Retorna a lista do Tipo de Parceria</returns>
        public List<TipoParceriaDto> GetTipoParceriaAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceTipoParceria}"));
            return Get<List<TipoParceriaDto>>(requestUrl);
        }

        #endregion
    }
}