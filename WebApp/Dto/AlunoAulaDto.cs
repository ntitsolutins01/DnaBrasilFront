using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class AlunoAulaDto
    {
        public required string AlunoId { get; set; }
        public required string AulaId { get; set; }
        public int Progresso { get; set; }
    }
}
