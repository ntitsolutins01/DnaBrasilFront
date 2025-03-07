using System.Collections;
using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{ 
    /// <summary>
    /// Modalidade Client
    /// </summary>
    public partial class DnaApiClient
    {
	    private const string ResourceModalidade = "Modalidades";

        #region Main Methods

        /// <summary>
        /// Inclusão da Modalidade
        /// </summary>
        /// <param name="command">Objeto de inclusão da Modalidad</param>
        /// <returns>Id de Modalidade inserido</returns>
        public Task<long> CreateModalidade(ModalidadeModel.CreateUpdateModalidadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModalidade}"));
            return Post(requestUrl, command);
        }

        /// <summary>
        /// Alteração da Modalidade
        /// </summary>
        /// <param name="id">Id de alteração da Modalidade</param>
        /// <param name="command">Objeto de alteração da Modalidade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> UpdateModalidade(int id, ModalidadeModel.CreateUpdateModalidadeCommand command)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModalidade}/{id}"));
            return Put(requestUrl, command);
        }

        /// <summary>
        /// Exclusão da Modalidade
        /// </summary>
        /// <param name="id">Id de exclusão da Modalidade</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteModalidade(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModalidade}/{id}"));
            return Delete<bool>(requestUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Modalidade
        /// </summary>
        /// <param name="id">Id da Modalidade a ser buscada</param>
        /// <returns>Retorna o objeto da Modalidade</returns>
        public ModalidadeDto GetModalidadeById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModalidade}/{id}"));
            return Get<ModalidadeDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Modalidade cadastradas
        /// </summary>
        /// <returns>Retorna a Lista de Modalidade</returns>
        public List<ModalidadeDto> GetModalidadeAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModalidade}"));
            return Get<List<ModalidadeDto>>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Modalidades por Linha de Ação
        /// </summary>
        /// <param name="id">Id da linha de ação a ser buscada</param>
        /// <returns>Retorna a Lista de Modalidades</returns>
        public List<ModalidadeDto> GetModalidadesByLinhaAcaoId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModalidade}/LinhaAcao/{id}"));
            return Get<List<ModalidadeDto>>(requestUrl);
        }


        /// <summary>
        /// Busca Modalidades pelo Id do Profissional
        /// </summary>
        /// <param name="id">Id do profissional a ser buscado</param>
        /// <returns>Retorna a lista de Modalidades</returns>
        public List<ModalidadeDto> GetModalidadesByProfissionalId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceModalidade}/Profissional/{id}"));
            return Get<List<ModalidadeDto>>(requestUrl);
        }
        #endregion
    }
}