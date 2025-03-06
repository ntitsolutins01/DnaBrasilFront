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
        /// Inclusão de Fomento
        /// </summary>
        /// <param name="command">Objeto para inclusão de Fomento</param>
        /// <returns>Id de Fomento inserido</returns>
        public Task<long> CreateFomento(FomentoModel.CreateUpdateFomentoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        ///  Alteração de Fomento
        /// </summary>
        /// <param name="id">Id de alteração de Fomento</param>
        /// <param name="command">Objeto de alteração de Fomento</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateFomento(int id, FomentoModel.CreateUpdateFomentoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceFomento}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Fomento
        /// </summary>
        /// <param name="id">Id de exclusao de Fomento</param>
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
        /// <param name="id">Id de Fomento a ser buscado</param>
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
        /// Busca o o fomento pelo id da localidade atraves da tabela FomentoLocalidades
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