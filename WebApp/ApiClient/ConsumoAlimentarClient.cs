using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Consumo Alimentar Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceConsumoAlimentar = "ConsumosAlimentares";
        #region Main Methods

        /// <summary>
        /// Inclusão de Consumo Alimentar
        /// </summary>
        /// <param name="command">Objeto de inclusão de Consumo Alimentar</param>
        /// <returns>Id de Consumo Alimentar inserido</returns>
        public Task<long> CreateConsumoAlimentar(ConsumoAlimentarModel.CreateUpdateConsumoAlimentarCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConsumoAlimentar}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        ///  Alteração de Consumo Alimentar
        /// </summary>
        /// <param name="id">Id de alteração de Consumo Alimentar</param>
        /// <param name="command">Objeto de alteração de Consumo Alimentar</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateConsumoAlimentar(int id, ConsumoAlimentarModel.CreateUpdateConsumoAlimentarCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConsumoAlimentar}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Consumo Alimentar
        /// </summary>
        /// <param name="id">Id de exclusão de Consumo Alimentar</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteConsumoAlimentar(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConsumoAlimentar}/{id}"));
            return Delete<bool>(requestUrl);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Busca todos os Consumo Alimentares cadastrados
        /// </summary>
        /// <param name="id">Id de Consumo Alimentar a ser buscado</param>
        /// <returns>Retorna o objeto de Consumo Alimentar</returns>
        public ConsumoAlimentarDto GetConsumoAlimentarById(int? id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConsumoAlimentar}/{id}"));
            return Get<ConsumoAlimentarDto>(requestUrl);
        }

        /// <summary>
        /// Busca um único Consumo Alimentar
        /// </summary>
        /// <returns>Retorna a lista de Consumo Alimentar</returns>
        public List<ConsumoAlimentarDto> GetConsumoAlimentarAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConsumoAlimentar}"));
            return Get<List<ConsumoAlimentarDto>>(requestUrl);
        }
        #endregion
    }
}