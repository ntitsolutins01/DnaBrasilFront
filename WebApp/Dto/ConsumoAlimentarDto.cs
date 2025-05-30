namespace WebApp.Dto
{
    public class ConsumoAlimentarDto
    {
        public int Id { get; set; }
        public ProfissionalDto Profissional { get; set; }
        public EncaminhamentoDto? Encaminhamento { get; set; }
        public string Respostas { get; set; }
        public string? StatusConsumoAlimentar { get; set; }
    }
}
