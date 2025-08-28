using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Educacional Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceEducacional = "Educacionais";

        #region Main Methods
        ///// <summary>
        ///// Inclusão de Educacional
        ///// </summary>
        ///// <param name="command">Objeto para inclusão do Educacional</param>
        ///// <returns>Id do curso inserido</returns>
        //public Task<long> CreateEducacional(EducacionalModel.CreateUpdateEducacionalCommand command)
        //{
        //    var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
        //        $"{ResourceEducacional}"));
        //    return Post(requestUrl, command);
        //}

        ///// <summary>
        ///// Alteração de Educacional
        ///// </summary>
        ///// <param name="id">Id de alteração de Educacional</param>
        ///// <param name="command">Objeto de alteração de Educacional</param>
        ///// <returns>Retorna true ou false</returns>
        //public Task<bool> UpdateEducacional(int id, EducacionalModel.CreateUpdateEducacionalCommand command)
        //{
        //    var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
        //        $"{ResourceEducacional}/{id}"));
        //    return Put(requestUrl, command);
        //}

        /// <summary>
        /// Exclusão de Educacional
        /// </summary>
        /// <param name="id">Id de exclusão de Educacional</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteEducacional(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEducacional}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Educacional
        /// </summary>
        /// <param name="id">Id de Educacional a ser buscado</param>
        /// <returns>Retorna o objeto de Educacional</returns>
        public EducacionalDto GetEducacionalById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEducacional}/{id}"));
            return Get<EducacionalDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Educacionais cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Educacionais</returns>
        public List<EducacionalDto> GetEducacionaisAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEducacional}"));
            return Get<List<EducacionalDto>>(requestUrl);
        }

        #endregion
    }
}