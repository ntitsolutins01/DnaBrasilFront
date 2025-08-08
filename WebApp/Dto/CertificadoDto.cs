using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class CertificadoDto
    {
        public required int Id { get; set; }
        public required int FomentoId { get; set; }
        public required string NomeFomento { get; set; }
        public required string Nome { get; set; }
        public required string Url { get; set; }
        public SelectList? ListFomentos { get; set; }
        public bool Status { get; set; }
    }
}