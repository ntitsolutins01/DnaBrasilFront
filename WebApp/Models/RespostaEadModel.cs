using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class RespostaEadModel
    {
        public RespostaEadDto RespostaEad { get; set; }
        public List<RespostaEadDto> RespostasEad { get; set; }
        public SelectList ListTiposRespostasEad { get; set; }

        public class CreateUpdateRespostaEadCommand
        {
            public required int QuestaoId { get; set; }
            public required string TipoResposta { get; set; }
            public bool RespostaCerta { get; set; }
            public required string Resposta { get; set; }
            public required decimal ValorPesoResposta { get; set; }
        }
    }

}
