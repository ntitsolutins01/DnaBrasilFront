namespace WebApp.Dto
{
    public class EstudanteDto
    {
        public int Id { get; set; }
        public int AspNetUserId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Sexo { get; set; }
        public DateTime DtNascimento { get; set; }
        public string NomeMae { get; set; }
        public string NomePai { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string Cep { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string RedeSocial { get; set; }
        public string Url { get; set; }
        public bool Status { get; set; }
        public bool Habilitado { get; set; }
        public int Idade { get; set; }
    }
}
