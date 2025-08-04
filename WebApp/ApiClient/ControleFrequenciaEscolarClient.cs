using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Controle de Frequencia Escolar Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceControleFrequenciaEscolar = "ControlesFrequenciasEscolares";

        #region Main Methods

        /// <summary>
        /// Inclusão Controle de Frequencia Escolar
        /// </summary>
        /// <param name="command">Objeto de inclusão Controle de Frequencia Escolar</param>
        /// <returns>Id de Frequencia Escolar inserido</returns>
        public Task<long> CreateControleFrequenciaEscolar(ControleFrequenciaEscolarModel.CreateUpdateControleFrequenciaEscolarCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleFrequenciaEscolar}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração Controle de Frequencia Escolar
        /// </summary>
        /// <param name="id">Id de alteração de Frequencia Escolar</param>
        /// <param name="command">Objeto de alteração Controle de Frequencia Escolar</param>
        /// <returns></returns>
        public Task<bool> UpdateControleFrequenciaEscolar(int id, ControleFrequenciaEscolarModel.CreateUpdateControleFrequenciaEscolarCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleFrequenciaEscolar}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão  Controle de Frequencia Escolar
        /// </summary>
        /// <param name="id">Id de exclusao Controle de Frequencia Escolar</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteControleFrequenciaEscolar(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleFrequenciaEscolar}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Controle de Frequencia Escolar
        /// </summary>
        /// <param name="id">Id  Controle de Frequencia Escolar a ser buscada</param>
        /// <returns>Retorna o objeto de Frequencia Escolar</returns>
        public ControleFrequenciaEscolarDto GetControleFrequenciaEscolarById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleFrequenciaEscolar}/{id}"));
            return Get<ControleFrequenciaEscolarDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Controles de FrequenciasEscolares cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Frequencia Escolar</returns>
        public List<ControleFrequenciaEscolarDto> GetControlesFrequenciasEscolaresAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleFrequenciaEscolar}"));
            return Get<List<ControleFrequenciaEscolarDto>>(requestUrl);
        }

        #endregion

    }
}