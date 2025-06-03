using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// AlunoCertificado Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceAlunoCertificado = "AlunosCertificados";

        #region Main Methods



        /// <summary>
        /// Alteração de AlunoCertificado
        /// </summary>
        /// <param name="id">Id de alteração de AlunoCertificado</param>
        /// <param name="command">Objeto de alteração de AlunoCertificado</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateAlunoCertificado(int id, AlunoCursoCertificadoModel.CreateUpdateAlunoCertificadoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunoCertificado}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de AlunoCertificado
        /// </summary>
        /// <param name="id">Id de exclusão de AlunoCertificado</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteAlunoCertificado(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunoCertificado}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única AlunoCertificado
        /// </summary>
        /// <param name="id">Id da AlunoCertificado a ser buscada</param>
        /// <returns>Retorna o objeto de AlunoCertificado</returns>
        public AlunoCertificadoDto GetAlunoCertificadoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunoCertificado}/{id}"));
            return Get<AlunoCertificadoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as AlunosCertificados cadastradas
        /// </summary>
        /// <returns>Retorna a lista de AlunoCertificado</returns>
        public List<AlunoCertificadoDto> GetAlunosCertificadosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceAlunoCertificado}"));
            return Get<List<AlunoCertificadoDto>>(requestUrl);
        }

        #endregion
    }
}