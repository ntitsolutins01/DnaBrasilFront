using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Configuraçao Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceConfiguracaoSistema = "ConfiguracaoSistema";
        #region Main Methods

        /// <summary>
        /// inclusão de Modulo
        /// </summary>
        /// <param name="command">Objeto de inclusão da Modulo</param>
        /// <returns>Retorna Id de novo Modulo</returns>
        public Task<long> CreateModulo(ConfiguracaoSistemaModel.CreateUpdateModuloCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConfiguracaoSistema}/Modulo"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Modulo
        /// </summary>
        /// <param name="id">Id de alteração de Modulo</param>
        /// <param name="command">Objeto de alteração de Modulo</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateModulo(int id, ConfiguracaoSistemaModel.CreateUpdateModuloCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConfiguracaoSistema}/Modulo/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Modulo
        /// </summary>
        /// <param name="id">Id de exclusao de Modulo</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteModulo(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConfiguracaoSistema}/Modulo/{id}"));
            return Delete<bool>(requestUrl);
        }

        /// <summary>
        /// Inclusão de Funcionalidade
        /// </summary>
        /// <param name="command">Objeto de inclusão de Funcionalidade</param>
        /// <returns>Retorna Id de nova Funcionalidade</returns>
        public Task<long> CreateFuncionalidade(FuncionalidadeModel.CreateUpdateFuncionalidadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConfiguracaoSistema}/Funcionalidade"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Funcionalidade
        /// </summary>
        /// <param name="id">Id de alteração de Funcionalidade</param>
        /// <param name="command">Objeto de alteração de Funcionalidade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateFuncionalidade(int id, FuncionalidadeModel.CreateUpdateFuncionalidadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConfiguracaoSistema}/Funcionalidade/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Funcionalidade
        /// </summary>
        /// <param name="id">Id de exclusao de Funcionalidade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteFuncionalidade(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConfiguracaoSistema}/Funcionalidade/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca todos os Modulos cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Modulos</returns>
        public List<ModuloDto> GetModulosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConfiguracaoSistema}/Modulos"));
            return Get<List<ModuloDto>>(requestUrl);
        }

        /// <summary>
        ///  Busca um único Modulo
        /// </summary>
        /// <param name="id">Id de Modulo a ser buscado</param>
        /// <returns>Retorna o objeto de Modulo</returns>
        public ModuloDto GetModuloById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConfiguracaoSistema}/Modulo/{id}"));
            return Get<ModuloDto>(requestUrl);
        }

        /// <summary>
        /// Busca uma única Funcionalidade
        /// </summary>
        /// <param name="id">Id de Funcionalidade a ser buscada</param>
        /// <returns>Retorna o objeto de Funcionalidade</returns>
        public FuncionalidadeDto GetFuncionalidadeById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConfiguracaoSistema}/Funcionalidade/{id}"));
            return Get<FuncionalidadeDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Funcionalidades cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Funcionalidades</returns>
        public List<FuncionalidadeDto> GetFuncionalidadesAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceConfiguracaoSistema}/Funcionalidades"));
            return Get<List<FuncionalidadeDto>>(requestUrl);
        }

        #endregion
    }
}