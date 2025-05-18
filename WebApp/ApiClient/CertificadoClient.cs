using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Certificado Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceCertificado = "Certificados";

        #region Main Methods

        /// <summary>
        ///  Inclusão de Certificado
        /// </summary>
        /// <param name="command">Objeto para inclusão de Certificado</param>
        /// <returns>Id de Certificado inserido</returns>
        public Task<long> CreateCertificado(CertificadoModel.CreateCertificadoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCertificado}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração de Certificado
        /// </summary>
        /// <param name="id">Id de alteração de Certificado</param>
        /// <param name="command">Objeto de alteração de Certificado</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateCertificado(int id, CertificadoModel.UpdateCertificadoCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCertificado}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão de Certificado
        /// </summary>
        /// <param name="id">Id de exclusão de Certificado</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteCertificado(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCertificado}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Certificado
        /// </summary>
        /// <param name="id">Id de Certificado a ser buscado</param>
        /// <returns>Retorna o objeto de Certificado</returns>
        public CertificadoDto GetCertificadoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCertificado}/{id}"));
            return Get<CertificadoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Certificados cadastrados
        /// </summary>
        /// <returns>Retorna a lista de Certificado</returns>
        public List<CertificadoDto> GetCertificadosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCertificado}"));
            return Get<List<CertificadoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Certificados por AlunoId 
        /// </summary>
        /// <param name="tipoCertificadoId">Id do  certificado</param>
        /// <returns>Retorna a lista por Certificado</returns>
        public List<CertificadoDto> GetCertificadosByAlunoId(int alunoId)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceCertificado}/Aluno/{alunoId}"));
            return Get<List<CertificadoDto>>(requestUrl);
        }

        #endregion
    }
}