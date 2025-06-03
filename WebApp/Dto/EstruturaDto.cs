using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class EstruturaDto
    {
        public required int Id { get; set; }
        public required LocalidadeDto Localidade { get; set; }
        public required int LocalidadeId { get; set; }
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public bool Status { get; set; }
        public SelectList ListLocalidades { get; set; }
    }
}
