using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using WebApp.Dto;

namespace WebApp.Models
{
	public class LaudoModel
	{
		public LaudoDto Laudo { get; set; }
		public AlunoDto Aluno { get; set; }
		public ProfissionalDto Profissional { get; set; }
        public DesempenhoDto Desempenho { get; set; }
        public EncaminhamentoDto EncaminhamentoImc { get; set; }
        public EncaminhamentoDto EncaminhamentoSaudeBucal { get; set; }
        public EncaminhamentoDto EncaminhamentoConsumoAlimentar { get; set; }
        public ModalidadeDto Modalidade { get; set; }
        public List<EncaminhamentoDto> ListQualidadeDeVida { get; set; }
        public List<EncaminhamentoDto> ListVocacional { get; set; }
        public TalentoEsportivoDto TalentoEsportivo { get; set; }
        public PaginatedListDto<LaudoDto> Laudos { get; set; }
		public List<QuestionarioDto> ListQuestionarioVocacional { get; set; }
		public List<QuestionarioDto> ListQuestionarioConsumoAlimentar { get; set; }
		public List<QuestionarioDto> ListQuestionarioQualidadeVida { get; set; }
		public List<QuestionarioDto> ListQuestionarioSaudeBucal { get; set; }
		public string FomentoId { get; set; }
		public SelectList ListFomentos { get; set; }
		public string LocalidadeId { get; set; }
		public SelectList ListLocalidades { get; set; }
		public string EstadoId { get; set; }
		public SelectList ListEstados { get; set; }
		public string MunicipioId { get; set; }
		public SelectList ListMunicipios { get; set; }
        public string AlunoId { get; set; }
        public SelectList ListAlunos { get; set; }
        public SelectList ListProfissionais { get; set; }
        public string ProfissionalId { get; set; }
        public string TipoLaudoId { get; set; }
        public SelectList ListTiposLaudos { get; set; }
        public SaudeDto Saude { get; set; }
        public VocacionalDto Vocacional { get; set; }
        public ConsumoAlimentarDto ConsumoAlimentar { get; set; }
        public QualidadeVidaDto QualidadeVida { get; set; }
        public SaudeBucalDto SaudeBucal { get; set; }
        public SelectList ListDeficiencias { get; set; }
        public string DeficienciaId { get; set; }
        public LaudosFilterDto SearchFilter { get; set; }

        public class CreateUpdateLaudoCommand
		{
			public int Id { get; set; }
            public required int AlunoId { get; set; }
            public int? SaudeId { get; set; }
            public int? VocacionalId { get; set; }
            public int? ConsumoAlimentarId { get; set; }
            public int? QualidadeDeVidaId { get; set; }
            public int? SaudeBucalId { get; set; }
            public int? TalentoEsportivoId { get; set; }
            public string? StatusLaudo { get; set; }
            public int? ModalidadeId { get; set; }
            
            
        }
	}
    public class SearchFilterDto
    {
        public string MunicipioId { get; set; }
        public string FomentoId { get; set; }
        public string LocalidadeId { get; set; }
        public string Sexo { get; set; }
        public string DeficienciaId { get; set; }
        public string Estado { get; set; }
        public string Etnia { get; set; }
    }
}