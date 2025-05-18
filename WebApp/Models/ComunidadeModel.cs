using WebApp.Dto;

namespace WebApp.Models
{
    public class ComunidadeModel
    {
        public ComunidadeDto Comunidade { get; set; }
        public List<ComunidadeDto> MetricasImc { get; set; }

        public class CreateUpdateComunidadeCommand
        {
            public int Id { get; set; }
            public string? NomeComunidade { get; set; }
            public bool Status { get; set; } = true;
        }
    }

}
