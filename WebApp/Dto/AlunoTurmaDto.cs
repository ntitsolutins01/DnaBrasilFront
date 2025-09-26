namespace WebApp.Dto
{
    public class AlunoTurmaDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public bool Status { get; set; }
        public bool Habilitado { get; set; }
        public string? SerieTurma { get; set; }
        public string? SerieId { get; set; }
        public string? EtapaId { get; set; }
        public string? SerieNome { get; set; }
    }
}