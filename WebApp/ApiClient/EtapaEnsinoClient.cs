using System.Collections;
using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
    /// <summary>
    /// EtapaEnsino Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceEtapaEnsino = "EtapasEnsino";
        #region Main Methods

        ///// <summary>
        ///// Inclusão de Etapas de Ensino
        ///// </summary>
        ///// <param name="command">Objeto de inclusão de Etapas de Ensino</param>
        ///// <returns>Id de EtapaEnsino inserido</returns>
        //public Task<long> CreateEtapaEnsino(EtapaEnsinoModel.CreateUpdateEtapaEnsinoCommand command)
        //{
        //    var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
        //        $"{ResourceEtapaEnsino}"));
        //    return Post(requestUrl, command);
        //}

        ///// <summary>
        ///// Alteração de Etapas de Ensino
        ///// </summary>
        ///// <param name="id">Id de alteração de Etapas de Ensino</param>
        ///// <param name="command">Objeto de alteração de Etapas de Ensino</param>
        ///// <returns>Retorna true ou false</returns>
        //public Task<bool> UpdateEtapaEnsino(int id, EtapaEnsinoModel.CreateUpdateEtapaEnsinoCommand command)
        //{
        //    var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
        //        $"{ResourceEtapaEnsino}/{id}"));
        //    return Put(requestUrl, command);
        //}

        ///// <summary>
        ///// Exclusão de Etapas de Ensino
        ///// </summary>
        ///// <param name="id">Id de Exclusão de Etapas de Ensino</param>
        ///// <returns>Retorna true ou false</returns>
        //public Task<bool> DeleteEtapaEnsino(int id)
        //{
        //    var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
        //        $"{ResourceEtapaEnsino}/{id}"));
        //    return Delete<bool>(requestUrl);
        //}

        #endregion

        #region Methods

        /// <summary>
        /// Busca uma única Etapas de Ensino
        /// </summary>
        /// <param name="id">Id de Etapas de Ensino a ser buscada</param>
        /// <returns>Retorna o objeto de Etapas de Ensino</returns>
        public EtapaEnsinoDto GetEtapaEnsinoById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEtapaEnsino}/{id}"));
            return Get<EtapaEnsinoDto>(requestUrl);
        }

        /// <summary>
        /// Busca todas as Etapas de Ensino cadastradas
        /// </summary>
        /// <returns>Retorna a lista de Etapas de Ensino</returns>
        public List<EtapaEnsinoDto> GetEtapasEnsinoAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEtapaEnsino}"));
            return Get<List<EtapaEnsinoDto>>(requestUrl);
        }

        /// <summary>
        /// Busca uma lista de Etapas por Localidade
        /// </summary>
        /// <param name="id">Id da localidade</param>
        /// <returns>Retorna uma lista de Etapas</returns>
        public List<EtapaEnsinoDto> GetEtapasByLocalidadeId(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceEtapaEnsino}/Localidade/{id}"));
            return Get<List<EtapaEnsinoDto>>(requestUrl);
        }

        #endregion
    }
}