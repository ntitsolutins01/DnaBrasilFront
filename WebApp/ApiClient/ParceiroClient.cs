using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    public partial class DnaApiClient
    {
        /// <summary>
        /// Parceiro Client
        /// </summary>

        private const string ResourceParceiro = "Parceiros";

        #region Main Methods

        /// <summary>
        /// Inclusão de Parceiro
        /// </summary>
        /// <param name="command">Objeto de inclusão de Parceiro</param>
        /// <returns>Id de Perceiro inserido</returns>
        public Task<long> CreateParceiro(ParceiroModel.CreateUpdateParceiroCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceParceiro}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Parceiro
        /// </summary>
        /// <param name="id">Id de alteração de Parceiro</param>
        /// <param name="command">Objeto de alteração de Parceiro</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateParceiro(int id, ParceiroModel.CreateUpdateParceiroCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceParceiro}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Parceiro
        /// </summary>
        /// <param name="id">Id de exclusão de Parceiro</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteParceiro(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceParceiro}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Parceiro
        /// </summary>
        /// <param name="id">Id de Parceiro a ser buscado</param>
        /// <returns>Retorna o objeto de Parceiro</returns>
        public async Task<ParceiroDto> GetParceiroById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceParceiro}/{id}"));
            return Get<ParceiroDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Parceiros cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Parceiros</returns>
        public List<ParceiroDto> GetParceiroAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceParceiro}"));
            return Get<List<ParceiroDto>>(requestUrl);
        }

        /// <summary>
        /// Busca Parceiro por id de Usuário 
        /// </summary>
        /// <param name="aspNetUserId">aspNetUserId</param>
        /// <returns>retorna id de usuario</returns>
        public ParceiroDto GetParceiroByAspNetUserId(string aspNetUserId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceParceiro}/AspNetUser/{aspNetUserId}"));
            return Get<ParceiroDto>(requestUrl);
        }

        #endregion

    }
}