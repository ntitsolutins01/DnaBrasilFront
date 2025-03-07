using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Fomento Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceFomento = "Fomentos";

        #region Main Methods

        /// <summary>
        /// Inclusão do Fomento
        /// </summary>
        /// <param name="command">Objeto para inclusão do Fomento</param>
        /// <returns>Id do Fomento inserido</returns>
        public Task<long> CreateFomento(FomentoModel.CreateUpdateFomentoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        ///  Alteração do Fomento
        /// </summary>
        /// <param name="id">Id de alteração do Fomento</param>
        /// <param name="command">Objeto de alteração do Fomento</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateFomento(int id, FomentoModel.CreateUpdateFomentoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão do fomento
        /// </summary>
        /// <param name="id">Id de exclusão do fomento</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteFomento(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Fomento
        /// </summary>
        /// <param name="id">Id do Fomento a ser buscado</param>
        /// <returns>Retorna um objeto de Fomento</returns>
        public FomentoDto GetFomentoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}/{id}"));
            return Get<FomentoDto>(requestUrl);
        }

        /// <summary>
        ///  Busca Fomento por Localidade Id 
        /// </summary>
        /// <param name="id">Id que busca Fomento por Localidade</param>
        /// <returns>retona a lista de Fomento</returns>
        public FomentoDto GetFomentoByLocalidadeId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}/Localidade/{id}"));
            return Get<FomentoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Fomentos cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Fomentos</returns>
        public List<FomentoDto> GetFomentosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}"));
            return Get<List<FomentoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca fomento de localidades pelo id da localidade
        /// </summary>
        /// <param name="id">Id da localidade</param>
        /// <returns>Retorna o objeto do fomento</returns>
        public FomentoDto GetFomentoLocalidadesByLocalidadeId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}/Fomento/Localidade/{id}"));
            return Get<FomentoDto>(requestUrl);
        }

        #endregion
    }
}