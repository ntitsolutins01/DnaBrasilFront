namespace WebApp.Dto
{
    public class TotalizadorDesempenhoDto
    {
        public Dictionary<string, decimal>? ValorTotalizadorDesempenhoMasculino { get; set; }
        public Dictionary<string, decimal>? ValorTotalizadorDesempenhoFeminino { get; set; }
        public Dictionary<string, decimal>? PercTotalizadorDesempenhoMasculino { get; set; }
        public Dictionary<string, decimal>? PercTotalizadorDesempenhoFeminino { get; set; }
        public Dictionary<string, decimal>? PercDesempenho { get; set; }
    }
}
