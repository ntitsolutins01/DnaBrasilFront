using WebApp.Dto;
using WebApp.Models;

namespace WebApp.ApiClient
{
	/// <summary>
	/// Controle de Material Client
	/// </summary>
	public partial class DnaApiClient
	{
		private const string ResourceControleMaterial = "ControlesMateriais";

        #region Main Methods

        /// <summary>
        /// Inclusão do Controle de Material
        /// </summary>
        /// <param name="command">Objeto de inclusão do Controle de Material</param>
        /// <returns>Id do Controle de Material inserido</returns>
        public Task<long> CreateControleMaterial(ControleMaterialModel.CreateUpdateControleMaterialCommand command)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceControleMaterial}"));
			return Post(requestUrl, command);
		}

        /// <summary>
        /// Alteração do Controle de Material
        /// </summary>
        /// <param name="id">Id de alteração do Controle de Material</param>
        /// <param name="command">Objeto de alteração do Controle de Material</param>
        /// <returns>Retorna  true false</returns>
        public Task<bool> UpdateControleMaterial(int id, ControleMaterialModel.CreateUpdateControleMaterialCommand command)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceControleMaterial}/{id}"));
			return Put(requestUrl, command);
		}

        /// <summary>
        /// Exclusão do Controle de Material
        /// </summary>
        /// <param name="id">Id de exclusao do Controle de Material</param>
        /// <returns>Retorna true ou false</returns>
        public Task<bool> DeleteControleMaterial(int id)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceControleMaterial}/{id}"));
			return Delete<bool>(requestUrl);
		}

        #endregion

        #region Methods

        /// <summary>
        /// Busca um único Controle de Material
        /// </summary>
        /// <param name="id">Id do Controle de Material a ser buscada</param>
        /// <returns>Retorna o objeto do Controle de Material</returns>
        public ControleMaterialDto GetControleMaterialById(int id)
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceControleMaterial}/{id}"));
			return Get<ControleMaterialDto>(requestUrl);
		}

        /// <summary>
        /// Busca todos os Controles de Materiais cadastradas
        /// </summary>
        /// <returns>Retorna a lista do Controle de Material</returns>
        public List<ControleMaterialDto> GetControlesMateriaisAll()
		{
			var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				$"{ResourceControleMaterial}"));
			return Get<List<ControleMaterialDto>>(requestUrl);
		}

		//public ControleMaterialDto GetControleMaterialByAlunoIdDisciplinaId(int alunoId, int disciplinaId)
		//{
		//	var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
		//		$"{ResourceControleMaterial}/Aluno/{alunoId}/Disciplina/{disciplinaId}"));
		//	return Get<ControleMaterialDto>(requestUrl);
		//}

		#endregion


	}
}