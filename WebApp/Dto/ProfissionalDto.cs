namespace WebApp.Dto
{
    public class ProfissionalDto
    {
        public int Id { get; set; }
        public string? AspNetUserId { get; set; }
        public string? AspNetRoleId { get; set; }
        public required string Nome { get; set; }
        public string? DtNascimento { get; set; }
        public required string Email { get; set; }
        public string? Sexo { get; set; }
        public required string CpfCnpj { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public string? Endereco { get; set; }
        public int? Numero { get; set; }
        public string? Cep { get; set; }
        public string? Bairro { get; set; }
        public string? Uf { get; set; }
        public int? MunicipioId { get; set; }
        public int? LocalidadeId { get; set; }
        public string? Localidade { get; set; }
        public bool Status { get; set; }
        public bool Habilitado { get; set; }
        public List<ModalidadeDto>? Modalidades { get; set; }
        public string? Perfil { get; set; }
        public string? Cargo { get; set; }
        public string? ModalidadesIds { get; set; }
        public bool PossuiAlunosVinculados { get; set; }
        public int? QuantidadeAlunosVinculados { get; set; }
    }
}
