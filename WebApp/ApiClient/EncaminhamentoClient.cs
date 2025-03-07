using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
	/// <summary>
	/// Encaminhamento Client
	/// </summary>
	public partial class DnaApiClient
	{
		private const string ResourceEncaminhamento = "Encaminhamentos";

        #region Main Methods

        /// <summary>
        /// Inclusão de Encaminhamento
        /// </summary>
        /// <param name="command">Objeto de inclusão da Encaminhamento</param>
        /// <returns>Id de Encaminhamento inserido</returns>
        public Task<long> CreateEncaminhamento(EncaminhamentoModel.CreateUpdateEncaminhamentoCommand command)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceEncaminhamento}"));
			return Post(requestUrl, command);
		}

        /// <summary>
        /// Alteração de Encaminhamento
        /// </summary>
        /// <param name="id">Id de alteração da Encaminhamento</param>
        /// <param name="command">Objeto de alteração da Encaminhamento</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateEncaminhamento(int id, EncaminhamentoModel.CreateUpdateEncaminhamentoCommand command)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceEncaminhamento}/{id}"));
			return Put(requestUrl, command);
		}

        /// <summary>
        /// Exclusão de Encaminhamento
        /// </summary>
        /// <param name="id">Id de exclusão da Encaminhamento</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteEncaminhamento(int id)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceEncaminhamento}/{id}"));
			return Delete<bool>(requestUrl);
		}

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Encaminhamento
        /// </summary>
        /// <param name="id">Id da Encaminhamento a ser buscada</param>
        /// <returns>Retorna o objeto da Encaminhamento</returns>
        public EncaminhamentoDto GetEncaminhamentoById(int id)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceEncaminhamento}/{id}"));
			return Get<EncaminhamentoDto>(requestUrl);
		}

        /// <summary>
        /// Busca todos os Encaminhamentos cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Encaminhamentos</returns>
        public List<EncaminhamentoDto> GetEncaminhamentosAll()
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceEncaminhamento}"));
			return Get<List<EncaminhamentoDto>>(requestUrl);
		}

        /// <summary>
        /// Busca todos os Encaminhamentos por tipo Laudo
        /// </summary>
        /// <param name="id">Id do tipo laudo</param>
        /// <returns>Retorna a lista de Encaminhamentos por Laudo</returns>
        public List<EncaminhamentoDto> GetEncaminhamentosByTipoLaudoId(int id)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceEncaminhamento}/TipoLaudo/{id}"));
			return Get<List<EncaminhamentoDto>>(requestUrl);
		}

		#endregion


	}
}