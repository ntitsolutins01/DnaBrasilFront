using WebApp.Models;

namespace WebApp.Dto
{
	public class DashboardDto
	{
		public int AvaliacoesDna { get; set; }
		public int LaudosAndamentos { get; set; }
		public int LaudosFinalizados { get; set; }
		public int CadastrosMasculinos { get; set; }
		public int CadastrosFemininos { get; set; }
		public int AlunosCadastrados { get; set; }
		public int LaudosMasculinos { get; set; }
		public int LaudosFemininos { get; set; }
		public int[]? ListPresencasAnual { get; set; }
		public int[]? ListFaltasAnual { get; set; }
        public StatusLaudosDto? StatusLaudos { get; set; }
		public int Ultimos3Meses { get; set; }
		public int Ultimos6Meses { get; set; }
		public int Em1Ano { get; set; }
        public TotalizadorSexoSaudeDto? ListTotalizadorSaudeSexo { get; set; }
        public TotalizadorTalentoDto? ListTotalizadorTalento { get; set; }
        public List<DataGrafico> ListPercTalento { get; set; }
        public List<string>? ListPercTalentoCategorias { get; set; }
        public List<DataGrafico>? ListValorTalentoMasc { get; set; }
        public List<DataGrafico>? ListValorTalentoFem { get; set; }
        public TotalizadorQualidadeVidaDto? ListTotalizadorQualidadeVida { get; set; }
        public TotalizadorConsumoAlimentarDto? ListTotalizadorConsumoAlimentar { get; set; }
        public TotalizadorSaudeBucalDto? ListTotalizadorSaudeBucal { get; set; }
        public TotalizadorVocacionalDto? ListTotalizadorVocacional { get; set; }
        public TotalizadorDesempenhoDto? ListTotalizadorDesempenho { get; set; }
        public TotalizadorDeficienciaDto? ListTotalizadorDeficiencia { get; set; }
        public List<DataGrafico> ListPercDeficiencia { get; set; }
        public List<string>? ListPercDeficienciaCategorias { get; set; }
        public List<DataGrafico>? ListValorDeficienciaMasc { get; set; }
        public List<DataGrafico>? ListValorDeficienciaFem { get; set; }

        public TotalizadorEtniaDto? ListTotalizadorEtnia { get; set; }
        public List<DataGrafico> ListPercEtnia { get; set; }
        public List<string>? ListPercEtniaCategorias { get; set; }
        public List<DataGrafico>? ListValorEtniaMasc { get; set; }
        public List<DataGrafico>? ListValorEtniaFem { get; set; }

        #region SearchFilter
        public string Sexo { get; set; }
		public string FomentoId { get; set; }
		public string Estado { get; set; }
		public string MunicipioId { get; set; }
		public string LocalidadeId { get; set; }
        public string DeficienciaId { get; set; }
        public string Etnia { get; set; }

        #endregion
	}
}
