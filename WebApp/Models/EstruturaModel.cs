using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class EstruturaModel
    {
        public EstruturaDto Estrutura { get; set; }
        public List<EstruturaDto> Estruturas { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public string LocalidadeId { get; set; }
        public SelectList ListLocalidades { get; set; }

        public class CreateUpdateEstruturaCommand
        {
            public int Id { get; set; }
            public required int LocalidadeId { get; set; }
            public required string Nome { get; set; }
            public string? Descricao { get; set; }
            public bool Status { get; set; }
        }
    }

}
