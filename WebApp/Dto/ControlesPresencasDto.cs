namespace WebApp.Dto
{
    public class ControlesPresencasDto
    {
        public int Id { get; set; }
        public int? EventoId { get; set; }
        public required string Controle { get; set; }
        public string? Justificativa { get; set; }
        public string? Data { get; set; }
        public bool Status { get; set; }
        public int Mes { get; internal set; }
    }
}