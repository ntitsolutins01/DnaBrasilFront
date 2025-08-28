namespace WebApp.Dto
{
    public class ControleFrequenciaEscolarDto
    {
        public int? Id { get; set; }
        public string Controle { get; set; }
        public string? AlunoId { get; set; }
        public string? SerieId { get; set; }
        public string? DisciplinaId { get; set; }
        public string? ProfissionalId { get; set; }
        public string DataFrequencia { get; set; }

    }
}
