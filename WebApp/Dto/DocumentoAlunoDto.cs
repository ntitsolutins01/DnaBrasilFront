namespace WebApp.Dto
{
    public class DocumentoAlunoDto
    {
        public required int Id { get; set; }
        public required string NomeAluno { get; set; }
        public required string NomeDocumento { get; set; }
        public required string Url { get; set; }
        public bool Status { get; set; }
    }
}
