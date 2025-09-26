using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Grau de Parentesco Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceGrauParentesco = "GrauParentescos";

        #region Main Methods

        /// <summary>
        /// Inclusão de Grau de Parentesco
        /// </summary>
        /// <param name="command">Objeto de inclusão da Grau de Parentesco</param>
        /// <returns>Id de GrauParentesco inserido</returns>
        public Task<long> CreateGrauParentesco(GrauParentescoModel.CreateUpdateGrauParentescoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceGrauParentesco}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        ///  Alteração de Grau de Parentesco
        /// </summary>
        /// <param name="id">Id de alteração da Grau de Parentesco</param>
        /// <param name="command">Objeto de alteração da Grau de Parentesco</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateGrauParentesco(int id, GrauParentescoModel.CreateUpdateGrauParentescoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceGrauParentesco}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Grau de Parentesco
        /// </summary>
        /// <param name="id">Id de exclusao da Grau de Parentesco</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteGrauParentesco(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceGrauParentesco}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca todas as GrauParentescos Cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Grau de Parentescos</returns>
        public List<GrauParentescoDto> GetGrauParentescosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceGrauParentesco}"));
            return Get<List<GrauParentescoDto>>(requestUrl);
        }

        /// <summary>
        ///  Busca um único grau de parentesco
        /// </summary>
        /// <param name="id">Id da Grau Parentesco a ser buscado</param>
        /// <returns>Retorna o objeto da Grau Parentesco</returns>
        public GrauParentescoDto GetGrauParentescoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceGrauParentesco}/{id}"));
            return Get<GrauParentescoDto>(requestUrl);
        }
        #endregion
    }
}