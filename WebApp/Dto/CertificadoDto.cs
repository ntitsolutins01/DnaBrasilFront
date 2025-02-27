namespace WebApp.Dto
{
    public class CertificadoDto
    {
        public required int Id { get; init; }
        public required int FomentoId { get; init; }
        public required string NomeFomento { get; init; }
        public required string ImagemFrente { get; init; }
        public string? ImagemVerso { get; init; }
        public string? NomeImagemFrente { get; init; }
        public string? NomeImagemVerso { get; init; }
        public required string HtmlFrente { get; init; }
        public required string HtmlVerso { get; init; }
        public bool Status { get; init; }
    }
}