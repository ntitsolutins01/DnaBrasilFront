using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Controle Material de Estoque e Saída Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceControleMaterialEstoqueSaida = "ControlesMateriaisEstoquesSaidas";

        #region Main Methods

        /// <summary>
        /// Inclusão do Material de Estoque e Saída
        /// </summary>
        /// <param name="command">Objeto de inclusão do Material de Estoque e Saída</param>
        /// <returns>Retorna id do Material de Estoque e Saída</returns>
        public Task<long> CreateControleMaterialEstoqueSaida(ControleMaterialEstoqueSaidaModel.CreateUpdateControleMaterialEstoqueSaidaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração do Material de Estoque e Saída
        /// </summary>
        /// <param name="id">Id do Material de Estoque e Saída</param>
        /// <param name="command">Objeto de alteração do Material de Estoque e Saída</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateControleMaterialEstoqueSaida(int id, ControleMaterialEstoqueSaidaModel.CreateUpdateControleMaterialEstoqueSaidaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão do Material de Estoque e Saída
        /// </summary>
        /// <param name="id">Id de exclusão do Material de Estoque e Saída</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteControleMaterialEstoqueSaida(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca Controle de Material de Estoque e Saída por ID
        /// </summary>
        /// <param name="id">Id de Controle de Material de Estoque e Saída por ID a ser buscada</param>
        /// <returns>Retorna o objeto de Controle de Material de Estoque e Saída por ID</returns>
        public ControleMaterialEstoqueSaidaDto GetControleMaterialEstoqueSaidaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}/{id}"));
            return Get<ControleMaterialEstoqueSaidaDto>(requestUrl);
        }

        /// <summary>
        /// Busca Todos os Controles de Materiais Estoques e Saídas cadastradas
        /// </summary>
        /// <returns>Retorna a Lista de Controles de Materiais Estoques e Saídas</returns>
        public List<ControleMaterialEstoqueSaidaDto> GetControlesMateriaisEstoquesSaidasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}"));
            return Get<List<ControleMaterialEstoqueSaidaDto>>(requestUrl);
        }

        /// <summary>
        /// Busca Controles de Materiais Estoques e Saídas por ID do Material
        /// </summary>
        /// <param name="materialId">Id do Material</param>
        /// <returns>Retorna a lista de Controles de Materiais Estoques e Saídas por ID do Material</returns>
        public List<ControleMaterialEstoqueSaidaDto> GetControlesMateriaisEstoquesSaidasByMaterialId(int materialId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceControleMaterialEstoqueSaida}/ControleMaterialEstoqueSaida/{materialId}"));
            return Get<List<ControleMaterialEstoqueSaidaDto>>(requestUrl);
        }
        #endregion
    }
}