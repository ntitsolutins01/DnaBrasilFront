

namespace WebApp.Dto
{
    public class CreateFotoEventoDto
    {
        public required int EventoId { get; set; }
        public required string NomeArquivo { get; set; }
        public required string Url { get; set; }
    }
}
