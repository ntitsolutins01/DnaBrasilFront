using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class AlunoCertificadoDto
    {
        public required int Id { get; set; }
        public required int AlunoId { get; set; }
        public required int CertificadoId { get; set; }
        public DateTimeOffset? Created { get; set; }
    }
}
