namespace WebApp.Dto
{
    public class TotalizadorQualidadeVidaDto
    {
        public Dictionary<string, decimal>? PercTotalizadorQualidadeVidaMasculino { get; set; }
        public Dictionary<string, decimal>? PercTotalizadorQualidadeVidaFeminino { get; set; }
        public Dictionary<string, decimal>? ValorTotalizadorQualidadeVidaMasculino { get; set; }
        public Dictionary<string, decimal>? ValorTotalizadorQualidadeVidaFeminino { get; set; }
        public Dictionary<string, decimal>? PercentualQualidade { get; set; }
    }
}
