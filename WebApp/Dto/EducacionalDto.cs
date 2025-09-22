using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class EducacionalDto
    {
        public int Id { get; set; }
        public required string ProfissionalId { get; set; }
        public required string AlunoId { get; set; }
        public required string Gabarito { get; set; }
        public required string Respostas { get; set; }
        public string? EncaminhamentoId { get; set; }
        public string? StatusEducacional { get; set; }
        public string? Imagem { get; set; }
        public string? NomeImagem { get; set; }
        public DateTimeOffset? DataCriacao { get; set; }
    }
}
