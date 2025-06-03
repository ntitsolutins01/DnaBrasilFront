using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class TextoLaudoModel
    {
        public TextoLaudoDto TextoLaudo { get; set; }
        public List<TextoLaudoDto> TextosLaudos { get; set; }
        public string TextoLaudoId { get; set; }
        public SelectList ListTextosLaudos { get; set; }

        public class CreateUpdateTextoLaudoCommand
        {
            public int Id { get; set; }
            public int? TipoLaudoId { get; init; }
            public string? Classificacao { get; init; }
            public decimal? PontoInicial { get; init; }
            public decimal? PontoFinal { get; init; }
            public string? Aviso { get; init; }
            public string? Texto { get; init; }
            public string? Sexo { get; set; }
            public int? Idade { get; set; }
            public int? Quadrante { get; set; }
        }
    }

}
