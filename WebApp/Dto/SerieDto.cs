namespace WebApp.Dto
{
    public class SerieDto
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Turma { get; set; }
        public required string NomeEtapaEnsino { get; set; }
        public required int LocalidadeId { get; set; }
        public required string NomeLocalidade { get; set; }
        public string? MunicipioEstado { get; set; }
        public bool Status { get; set; }
    }
}
