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
        public class CreateCertificadoCommand
        {
            public int Id { get; set; }
            public int FomentoId { get; set; }
            public string Nome { get; set; }
            public string Url { get; set; }
            public bool Status { get; set; } = true;
        }
        public class UpdateCertificadoCommand
        {
            public int Id { get; set; }
            public int FomentoId { get; set; }
            public string Nome { get; set; }
            public string Url { get; set; }
            public bool Status { get; set; } = true;
        }
    }
}