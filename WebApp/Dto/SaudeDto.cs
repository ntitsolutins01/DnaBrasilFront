namespace WebApp.Dto
{
    public class SaudeDto
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public int ProfissionalId { get; set; }
        public string? NomeProfissional { get; set; }
        public decimal? Altura { get; set; }
        public decimal? Massa { get; set; }
        public decimal? Envergadura { get; set; }
        public string? DataRealizacaoTeste { get; set; }
        public DateTime DtNascimento { get; set; }
        public string? Sexo { get; set; }
        public string? Imc { get; set; }
    }
}
