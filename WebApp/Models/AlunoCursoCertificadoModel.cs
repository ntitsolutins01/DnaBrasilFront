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
            public required int AlunoId { get; set; }
			public required string CursosId { get; set; }
        }

        public class CreateUpdateAlunoCertificadoCommand
        {
            public required int AlunoId { get; set; }
            public required string CertificadosId { get; set; }
        }
    }

}
