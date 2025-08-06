namespace WebApp.Dto
{
    public class ControleFrequenciaEscolarDto
    {
        public required int Id { get; set; }
        public required string Controle { get; set; }
        public string? AlunoId { get; set; }
        public string? SerieId { get; set; }
        public string? DisciplinaId { get; set; }
        public DateTimeOffset? Data { get; set; }

    }
}
