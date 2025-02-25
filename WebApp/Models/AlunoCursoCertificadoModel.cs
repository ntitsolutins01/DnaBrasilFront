using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
	public class AlunoCursoCertificadoModel
    {
        public AlunoCursoDto AlunoCurso { get; set; }
        public List<AlunoCursoDto> AlunosCursos { get; set; }
        public int AlunoId { get; set; }
        public SelectList ListAlunos { get; set; }
        public string CursoId { get; set; }
        public List<CursoDto> Cursos { get; set; }
        public SelectList ListCursos { get; set; }
        public string CertificadoId { get; set; }
        public List<CertificadoDto> Certificados { get; set; }
        public SelectList ListCertificados { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public string? LocalidadeId { get; set; }
        public SelectList ListLocalidades { get; set; }

        public class CreateUpdateAlunoCursoCommand
		{
			public int Id { get; set; }
            public required int AlunoId { get; set; }
			public required int CursoId { get; set; }
			public int Progresso { get; set; }
        }

        public class CreateUpdateAlunoCertificadoCommand
        {
            public int Id { get; set; }
            public required int AlunoId { get; set; }
            public required int CertificadoId { get; set; }
        }
    }

}
