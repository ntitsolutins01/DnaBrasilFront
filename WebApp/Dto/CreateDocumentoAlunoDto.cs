

namespace WebApp.Dto
{
    public class CreateDocumentoAlunoDto
    {
        public required int AlunoId { get; set; }
        public required string NomeDocumento { get; set; }
        public required string Url { get; set; }
    }
}
