namespace WebApp.Dto
{
    public class TotalizadorSaudeBucalDto
    {
        public Dictionary<string, decimal>? PercTotalizadorSaudeBucalMasculino { get; set; }
        public Dictionary<string, decimal>? PercTotalizadorSaudeBucalFeminino { get; set; }
        public Dictionary<string, decimal>? ValorTotalizadorSaudeBucalMasculino { get; set; }
        public Dictionary<string, decimal>? ValorTotalizadorSaudeBucalFeminino { get; set; }
        public Dictionary<string, decimal>? PercentualSaudeBucal { get; set; }
    }
}
