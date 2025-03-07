using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Categoria Client
    /// </summary>
    public partial class DnaApiClient
    {
	    private const string ResourceCategoria = "Categorias";

        #region Main Methods

        /// <summary>
        /// Inclusão de Categoria
        /// </summary>
        /// <param name="command">Objeto para inclusão de Categoria</param>
        /// <returns>Id de Categoria inserido</returns>
        public Task<long> CreateCategoria (CategoriaModel.CreateUpdateCategoriaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCategoria }"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Categoria
        /// </summary>
        /// <param name="id">Id de alteração de Categoria</param>
        /// <param name="command">Objeto de alteração de Categoria</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateCategoria (int id, CategoriaModel.CreateUpdateCategoriaCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCategoria }/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Categoria
        /// </summary>
        /// <param name="id">Id de exclusão de Categoria</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteCategoria (int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCategoria }/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Buscar uma única Categoria
        /// </summary>
        /// <param name="id">Id de Categoria a ser buscado</param>
        /// <returns>Retorna o objeto de Categoria</returns>
        public CategoriaDto GetCategoriaById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCategoria}/{id}"));
            return Get<CategoriaDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Categorias cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Categoria</returns>
        public List<CategoriaDto> GetCategoriasAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCategoria}"));
            return Get<List<CategoriaDto>>(requestUrl);
        }

        #endregion
    }
}