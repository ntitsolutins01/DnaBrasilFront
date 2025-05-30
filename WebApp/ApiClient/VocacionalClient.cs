using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Vocacional Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceVocacional = "Vocacionais";
        #region Main Methods

        /// <summary>
        /// Inclusão de Vocacional
        /// </summary>
        /// <param name="command">Objeto de inclusão de Vocacionail</param>
        /// <returns>Id de Vocacional inserido</returns>
        public Task<long> CreateVocacional(VocacionalModel.CreateUpdateVocacionalCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceVocacional}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Vocacional 
        /// </summary>
        /// <param name="id">Id de alteração da Vocacional</param>
        /// <param name="command">Objeto de alteração da Vocacional</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateVocacional(int id, VocacionalModel.CreateUpdateVocacionalCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceVocacional}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Vocacional
        /// </summary>
        /// <param name="id">Id de exclusão de Vocacional</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteVocacional(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceVocacional}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Busca um único Vocacional
        /// </summary>
        /// <param name="id">Id de Vocacional a ser buscado</param>
        /// <returns>Retorna o objeto de Vocacional</returns>
        public VocacionalDto GetVocacionalById(int? id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceVocacional}/{id}"));
            return Get<VocacionalDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Vocacional cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Vocacional</returns>
        public List<VocacionalDto> GetVocacionalAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceVocacional}"));
            return Get<List<VocacionalDto>>(requestUrl);
        }

        #endregion
    }
}