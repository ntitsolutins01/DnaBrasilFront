namespace WebApp.Dto
{
    public class QualidadeVidaDto
    {
        public int Id { get; set; }
        public ProfissionalDto Profissional { get; set; }
        public string? Encaminhamentos { get; set; }
        public string Respostas { get; set; }
        public string? StatusQualidadeDeVida { get; set; }
    }
}
