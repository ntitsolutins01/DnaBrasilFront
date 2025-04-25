using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
	public class AulaModel
	{
        public AulaDto Aula { get; set; }
        public List<AulaDto> Aulas { get; set; }
        public string ProfessorId { get; set; }
        public SelectList ListProfessores { get; set; }
        public string ModuloEadId { get; set; }
        public SelectList ListModulosEad { get; set; }
        public string CursoId { get; set; }
        public SelectList ListCursos { get; set; }
        public string TipoCursosId { get; set; }
        public SelectList ListTipoCursos { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }

		public class CreateUpdateAulaCommand
		{
			public int Id { get; set; }
			public required int ProfessorId { get; set; }
			public int ModuloEadId { get; set; }
            public required string Titulo { get; set; }
            public string? Descricao { get; set; }
            public string? Video { get; set; }
            public string? NomeVideo { get; set; }
            public bool Status { get; set; } = true;
            public string? Material { get; set; }
            public string? NomeMaterial { get; set; }
            public int? Ordem { get; set; }
        }
	}

}
