using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class TextoImagemQuestaoDto
    {
        public int Id { get; set; }
        public required int QuestaoEadId { get; set; }
        public int Ordem { get; set; }
        public string? Tipo { get; set; }
        public string? TextoImagem { get; set; }
    }
}
