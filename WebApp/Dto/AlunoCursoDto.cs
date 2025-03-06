using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class AlunoCursoDto
    {
        public required string AlunoId { get; set; }
        public required string CursoId { get; set; }
        public int Progresso { get; set; }
    }
}
