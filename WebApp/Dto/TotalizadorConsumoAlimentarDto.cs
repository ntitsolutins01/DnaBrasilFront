namespace WebApp.Dto
{
    public class TotalizadorConsumoAlimentarDto
    {
        public Dictionary<string, decimal>? PercTotalizadorConsumoAlimentarMasculino { get; set; }
        public Dictionary<string, decimal>? PercTotalizadorConsumoAlimentarFeminino { get; set; }
        public Dictionary<string, decimal>? ValorTotalizadorConsumoAlimentarMasculino { get; set; }
        public Dictionary<string, decimal>? ValorTotalizadorConsumoAlimentarFeminino { get; set; }
        public Dictionary<string, decimal>? PercentualConsumoAlimentar { get; set; }
    }
}
