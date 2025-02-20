using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class AlunoCursoDto
    {
        public required int Id { get; init; }
        public required int AlunoId { get; init; }
        public required int CursoId { get; init; }
        public int Progresso { get; init; }
    }
}
