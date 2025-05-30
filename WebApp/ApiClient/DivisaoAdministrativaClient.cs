using WebApp.Dto;

namespace WebApp.ApiClient
{
    /// <summary>
    /// Divisao Administrativa Client
    /// </summary>
    public partial class DnaApiClient
    {
        private const string ResourceDivisaoAdministrativa = "DivisoesAdministrativas";

        #region Methods

        /// <summary>
        /// Busca todos os Municipios por Id
        /// </summary>
        /// <param name="id">Id de Municipio a ser buscado</param>
        /// <returns>Retorna o objeto de Municipio</returns>
        public MunicipioDto GetMunicipioById(int id)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDivisaoAdministrativa}/Municipio/{id}"));
            return Get<MunicipioDto>(requestUrl);
        }

        /// <summary>
        /// Busca todos os Estados
        /// </summary>
        /// <returns>Retorna a lisa de Estados</returns>
        public List<EstadoDto> GetEstadosAll()
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDivisaoAdministrativa}/Estados"));
            return Get<List<EstadoDto>>(requestUrl);
        }

        /// <summary>
        ///  Busca todos os Municipios pela Uf
        /// </summary>
        /// <param name="uf">uf</param>
        /// <returns>Retorna um objeto de Municipio pela Uf</returns>
        public List<MunicipioDto> GetMunicipiosByUf(string uf)
        {
            var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                $"{ResourceDivisaoAdministrativa}/Municipios/{uf}"));
            return Get<List<MunicipioDto>>(requestUrl);
        }

        #endregion

    }
}