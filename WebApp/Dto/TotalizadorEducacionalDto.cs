namespace WebApp.Dto
{
    public class TotalizadorEducacionalDto
    {
        public Dictionary<string, decimal>? PercTotalizadorEducacionalMasculino { get; set; }
        public Dictionary<string, decimal>? PercTotalizadorEducacionalFeminino { get; set; }
        public Dictionary<string, decimal>? ValorTotalizadorEducacionalMasculino { get; set; }
        public Dictionary<string, decimal>? ValorTotalizadorEducacionalFeminino { get; set; }
        public Dictionary<string, decimal>? PercentualEducacional { get; set; }
    }
}
