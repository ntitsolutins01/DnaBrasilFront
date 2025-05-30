namespace WebApp.Dto
{
    public class SaudeBucalDto
    {
        public int Id { get; set; }
        public ProfissionalDto Profissional { get; set; }
        public EncaminhamentoDto? Encaminhamento { get; set; }
        public string Respostas { get; set; }
        public string? StatusSaudeBucal { get; set; }
    }
}
