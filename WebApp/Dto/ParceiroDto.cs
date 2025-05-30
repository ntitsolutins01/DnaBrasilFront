namespace WebApp.Dto
{
    public class ParceiroDto
    {
        public int Id { get; set; }
        public string? AspNetUserId { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string TipoPessoa { get; set; }
        public required string CpfCnpj { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public string? Cep { get; set; }
        public string? Endereco { get; set; }
        public int? Numero { get; set; }
        public string? Bairro { get; set; }
        public bool Status { get; set; }
        public bool Habilitado { get; set; }
        public List<AlunoDto>? Alunos { get; set; }
        public TipoParceriaDto? TipoParceria { get; set; }
        public int? MunicipioId { get; set; }
        public int EstadoId { get; set; }
        public string Uf { get; set; }
        public string TipoParceriaNome { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeContato { get; set; }
    }
}
