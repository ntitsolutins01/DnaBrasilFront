using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class SaudeModel
    {
        public SaudeDto Saude { get; set; }
        public List<SaudeDto> Saudes { get; set; }
        public string SaudeId { get; set; }
        public SelectList ListSaudes { get; set; }

        public class CreateUpdateSaudeCommand
        {
            public int Id { get; set; }
            public int? ProfissionalId { get; set; }
            public int? AlunoId { get; set; }
            public decimal? EnvergaduraSaude { get; set; }
            public decimal? MassaCorporalSaude { get; set; }
            public decimal? AlturaSaude { get; set; }
            public string StatusSaude { get; set; }
        }
    }

}
