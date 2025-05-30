namespace WebApp.Dto
{
    public class FotoEventoDto
    {
        public required int Id { get; set; }
        public required string NomeEvento { get; set; }
        public required string NomeArquivo { get; set; }
        public required string Url { get; set; }
        public bool Status { get; set; }
    }
}
