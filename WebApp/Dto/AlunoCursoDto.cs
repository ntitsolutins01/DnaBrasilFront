using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class AlunoCursoDto
    {
        public required int Id { get; set; }
        public required int AlunoId { get; set; }
        public required int CursoId { get; set; }
        public int Progresso { get; set; }
    }
}
