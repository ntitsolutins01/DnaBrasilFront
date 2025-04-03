namespace WebApp.Dto
{
    public class ModeloCarteirinhaDto
    {
        public int Id { get; set; }
        public int FomentoId { get; set; }
        public string NomeImagemFrente { get; set; }
        public string? UrlImagemFrente { get; set; }
        public string NomeImagemVerso { get; set; }
        public string? UrlImagemVerso { get; set; }
    }
}