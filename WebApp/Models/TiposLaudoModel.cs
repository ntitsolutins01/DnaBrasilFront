using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class TiposLaudoModel
    {
        public TiposLaudoDto TiposLaudo { get; set; }
        public List<TiposLaudoDto> TiposLaudos { get; set; }
        public string TiposLaudoId { get; set; }
        public SelectList ListTiposLaudos { get; set; }

        public class CreateUpdateTiposLaudoCommand
        {
            public string Nome { get; set; }
            public string Descricao { get; set; }
        }
    }

}
