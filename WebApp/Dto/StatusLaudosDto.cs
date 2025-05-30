namespace WebApp.Dto
{
    public class StatusLaudosDto
    {
        public int TotTalentoEsportivoFinalizado { get; set; }
        public int TotTalentoEsportivoAndamento { get; set; }
        public int TotSaudeFinalizado { get; set; }
        public int TotSaudeAndamento { get; set; }
        public int TotQualidadeDeVidaFinalizado { get; set; }
        public int TotQualidadeDeVidaAndamento { get; set; }
        public int TotVocacionalFinalizado { get; set; }
        public int TotVocacionalAndamento { get; set; }
        public int TotConsumoAlimentarFinalizado { get; set; }
        public int TotConsumoAlimentarAndamento { get; set; }
        public int TotSaudeBucalFinalizado { get; set; }
        public int TotSaudeBucalAndamento { get; set; }
        public double ProgressoSaude { get; set; }
        public double ProgressoTalentoEsportivo { get; set; }
        public double ProgressoQualidadeDeVida { get; set; }
        public double ProgressoVocacional { get; set; }
        public double ProgressoConsumoAlimentar { get; set; }
        public double ProgressoSaudeBucal { get; set; }
    }
}
