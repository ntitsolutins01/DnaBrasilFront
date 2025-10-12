using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class TextoImagemQuestaoModel
    {
        public TextoImagemQuestaoDto TextoImagemQuestao { get; set; }
        public List<TextoImagemQuestaoDto> TextoImagemQuestaos { get; set; }
        public SelectList ListTiposTextoImagemQuestaos { get; set; }

        public class CreateUpdateTextoImagemQuestaoCommand
        {
            public required int QuestaoEadId { get; set; }
            public string? Tipo { get; set; }
            public string? TextoImagem { get; set; }
            public int Ordem { get; set; }
        }
    }

}
