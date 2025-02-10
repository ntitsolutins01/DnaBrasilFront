namespace WebApp.Dto
{
    public class AlunoDto
    {
        public int Id { get; set; }
        public  string? AspNetUserId { get; set; }
        public  string Nome { get; set; }
        public  string Email { get; set; }
        public  string DtNascimento { get; set; }
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
        public string? NomeFoto { get; set; }
        public byte[]? ByteImage { get; set; }
        public bool Status { get; set; }
        public bool Habilitado { get; set; }
        public int Idade { get; set; }
        public List<ModalidadeDto>? ListModalidades { get; set; }
        public string? NomeMunicipio { get; set; }
        public int? DependenciaId { get; set; }
        public int? MatriculaId { get; set; }
        public string? NomeLocalidade { get; set; }
        public string? MunicipioEstado { get; set; }
        public SaudeDto Saude { get; set; }
        //public TalentoEsportivoDto TalentoEsportivo { get; set; }
        //public VocacionalDto Vocaiconal { get; set; }
        public byte[]? Image { get; set; }
        public byte[]? QrCode { get; set; }
        public string? Modalidades { get; set; }

        #region SearchFilter
        public string Sexo { get; set; }
        public string FomentoId { get; set; }
        public string Estado { get; set; }
        public string MunicipioId { get; set; }
        public string LocalidadeId { get; set; }
        public string DeficienciaId { get; set; }
        public string Etnia { get; set; }
        public string ProfissionalId { get; set; }
        public string ModalidadeLinhaAcao { get; set; }
        public string LinhaAcaoId { get; set; }
        public string NomePerfil { get; set; }
        public string DescricaoPerfil { get; set; }
        public string? ModalidadesIds { get; set; }

        #endregion
    }
}
