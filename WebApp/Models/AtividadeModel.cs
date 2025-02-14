using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;
using WebApp.Views;

namespace WebApp.Models
{
	public class AtividadeModel
	{
        public AtividadeDto Atividade { get; set; }
        public List<AtividadeDto> Atividades { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public int LocalidadeId { get; set; }
        public SelectList ListLocalidades { get; set; }
        public int EstruturaId { get; set; }
        public SelectList ListEstruturas { get; set; }
        public int LinhaAcaoId { get; set; }
        public SelectList ListLinhasAcoes { get; set; }
        public int AtividadeModalidadeId { get; set; }
        public SelectList ListAtividadesModalidades { get; set; }
        public int CategoriaId { get; set; }
        public SelectList ListCategorias { get; set; }
        public int ProfessorProfissionalId { get; set; }
        public SelectList ListProfessoresProfissionais { get; set; }

		public class CreateUpdateAtividadeCommand
		{
			public int Id { get; set; }
            public required int EstruturaId { get; set; }
            public required int LinhaAcaoId { get; set; }
            public required int CategoriaId { get; set; }
            public required int ModalidadeId { get; set; }
            public required string Turma { get; set; }
            public required string HrInicial { get; set; }
            public required string HrFinal { get; set; }
            public required int ProfissionalId { get; set; }
            public required int LocalidadeId { get; set; }
            public bool Status { get; set; }
            public required int QuantidadeAluno { get; set; }
            public required string DiasSemana { get; set; }
        }

        public class CreateUpdateAtividadeAlunosCommand
        {
            public required int AtividadeId { get; set; }
            public required string AlunosIds { get; set; }
        }
    }

}
