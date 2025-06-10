namespace WebApp.Dto
{
    public class AlunoCursoDto
    {
        public required string AlunoId { get; set; }
        public required string CursoId { get; set; }
        public string? CertificadoId { get; set; }
        public int Progresso { get; set; }
        public bool Status { get; set; }
    }
}
