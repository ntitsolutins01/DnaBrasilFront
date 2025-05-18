namespace WebApp.Dto
{
    public class EventoDto
    {
        public required int Id { get; set; }
        public required string EstadoId { get; set; }
        public required string Estado { get; set; }
        public required string MunicipioId { get; set; }
        public required string Municipio { get; set; }
        public required string LocalidadeId { get; set; }
        public required string Localidade { get; set; }
        public required string Titulo { get; set; }
        public string? Descricao { get; set; }
        public required string DataEvento { get; set; }
        public int TotalConvidado { get; set; }
        public int TotalAluno { get; set; }
        public bool Status { get; set; }
        public List<FotoEventoDto>? Fotos { get; set; }
    }
}
