using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class EducacionalDto
    {
        public int Id { get; set; }
        public required ProfissionalDto Profissional { get; set; }
        public required AlunoDto Aluno { get; set; }
        public required string Gabarito { get; set; }
        public required string Respostas { get; set; }
        public EncaminhamentoDto? Encaminhamento { get; set; }
        public string? StatusEducacional { get; set; }
        public string? Imagem { get; set; }
        public string? NomeImagem { get; set; }
    }
}
