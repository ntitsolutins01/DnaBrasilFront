namespace WebApp.Dto
{
    public class TotalizadorSexoSaudeDto
    {
        public Dictionary<string, decimal>? PercTotalizadorSaudeMasculino { get; set; }
        public Dictionary<string, decimal>? PercTotalizadorSaudeFeminino { get; set; }
        public Dictionary<string, decimal>? ValorTotalizadorSaudeMasculino { get; set; }
        public Dictionary<string, decimal>? ValorTotalizadorSaudeFeminino { get; set; }
        public Dictionary<string, decimal>? PercentualSaude { get; set; }
    }
}
