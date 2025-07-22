namespace WebApp.Dto
{
    public class AlunoCursoDto
    {
        public required string AlunoId { get; set; }
        public required string CursoId { get; set; }
        public string? CertificadoId { get; set; }
        public int Progresso { get; set; }
        public bool Status { get; set; }
        public string? Matricula { get; set; }
        public string? Idade { get; set; }
        public string? NomeAluno { get; set; }
        public string? LocalidadeMunicipioUf { get; set; }
    }
}
