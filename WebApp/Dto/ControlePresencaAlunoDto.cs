namespace WebApp.Dto
{
    public class ControlePresencaAlunoDto
    {
        public int AlunoId { get; set; }
        public required string NomeAluno { get; set; }
        public string? MunicipioEstado { get; set; }
        public string? NomeLocalidade { get; set; }
        public int LocalidadeId { get; set; }
        public int MunicipioId { get; set; }
        public byte[]? ByteImage { get; set; }
        public List<ControlePresencaDto>? ControlesPresencas { get; set; }
    }
}
