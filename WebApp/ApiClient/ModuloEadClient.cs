using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Modulo Ead Client
    /// </summary>
    public partial class DnaApiClient
    {
	    private const string ResourceModuloEad = "ModulosEad";

        #region Main Methods

        /// <summary>
        /// Inclusão de Modulo Ead
        /// </summary>
        /// <param name="command">Objeto de inclusão da Modulo Ead</param>
        /// <returns>Id do Modulo Ead inserido</returns>
        public Task<long> CreateModuloEad (ModuloEadModel.CreateUpdateModuloEadCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModuloEad}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Modulo Ead
        /// </summary>
        /// <param name="id">Id de Alteração da Modulo Ead</param>
        /// <param name="command">Objeto de alteração da Modulo Ead</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateModuloEad (int id, ModuloEadModel.CreateUpdateModuloEadCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModuloEad}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Modulo Ead
        /// </summary>
        /// <param name="id">Id de exclusão da Modulo Ead</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteModuloEad (int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModuloEad}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Busca um único Modulo Ead
        /// </summary>
        /// <param name="id">Id da Modulo Ead a ser buscada</param>
        /// <returns>Retorna o objeto da Modulo Ead</returns>
        public ModuloEadDto GetModuloEadById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModuloEad}/{id}"));
            return Get<ModuloEadDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Modulos Ead cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Modulos Ead</returns>
        public List<ModuloEadDto> GetModulosEadAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModuloEad}"));
            return Get<List<ModuloEadDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todos os modulo Ead por Curso id
        /// </summary>
        /// <param name="cursoId">Id por Curso</param>
        /// <returns>Retorna lista de Modulo Ead por Curso</returns>
        public List<ModuloEadDto> GetModulosEadAllByCursoId(int cursoId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModuloEad}/Curso/{cursoId}"));
            return Get<List<ModuloEadDto>>(requestUrl);
        }
        #endregion
    }
}