using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
	public class AlunoCursoModel
	{
        public AlunoCursoDto AlunoCurso { get; set; }
        public List<AlunoCursoDto> AlunosCursos { get; set; }
        public string AlunoId { get; set; }
        public SelectList ListAlunos { get; set; }
        public string CursoId { get; set; }
        public SelectList ListCursos { get; set; }

		public class CreateUpdateAlunoCursoCommand
		{
			public int Id { get; set; }
            public required int AlunoId { get; set; }
			public required int CursoId { get; set; }
			public int Progresso { get; set; }
        }
	}

}
