using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class CertificadoModel
    {
        public CertificadoModel()
        {
            Certificados = new List<CertificadoDto>();
        }
        public CertificadoDto Certificado { get; set; }
        public List<CertificadoDto> Certificados { get; set; }
        public string FomentoId { get; set; }
        public SelectList ListFomentos { get; set; }
        public class CreateUpdateCertificadoCommand
        {
            public int Id { get; set; }
            public int FomentoId { get; set; }
            public string ImagemFrente { get; set; }
            public string? ImagemVerso { get; set; }
            public string NomeImagemFrente { get; set; }
            public string? NomeImagemVerso { get; set; }
            public string HtmlFrente { get; set; }
            public string HtmlVerso { get; set; }
            public bool Status { get; set; } = true;
        }
    }
}