using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class ControleMaterialModel
    {
        public ControleMaterialDto ControleMaterial { get; set; }
        public List<ControleMaterialDto> ControlesMateriais { get; set; }
        public SelectList ListLinhasAcoes { get; set; }
        public string LinhaAcaoId { get; set; }



        public class CreateUpdateControleMaterialCommand
        {
            public required int Id { get; set; }
            public int LinhaAcaoId { get; set; }
            public required string Descricao { get; set; }
            public required string UnidadeMedida { get; set; }
            public required int Quantidade { get; set; }
            public int? Saida { get; set; }
            public int? Disponivel { get; set; }
            public bool Status { get; set; } = true;

        }
    }

}
