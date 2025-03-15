namespace WebApp.Dto
{
    public class ArquivosInventarioDto
    {
        public required int Id { get; set; }
        public required int InventarioId { get; set; }
        public string? PathArquivo { get; set; }
        public string? NomeArquivo { get; set; }
    }
}
