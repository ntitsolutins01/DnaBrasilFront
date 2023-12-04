namespace WebApp.Dto
{
    public class ProfissionalDto
    {
        public int Id { get; set; }
        public required int AspNetUserId { get; set; }
        public required string Nome { get; set; }
        public DateTime DtNascimento { get; set; }
        public required string Email { get; set; }
        public required string Sexo { get; set; }
        public required string Cpf { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public string? Endereco { get; set; }
        public int? Numero { get; set; }
        public string? Cep { get; set; }
        public string? Bairro { get; set; }
        public bool Status { get; set; } = true;
        public MunicipioDto? Municipio { get; set; }
        public List<AmbienteDto>? Ambientes { get; init; }
        public bool? Habilitado { get; set; }
        public List<ProfissionalDto>? Profissionais { get; set; }

    }
}
