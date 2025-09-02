namespace WebApp.Dto
{
    public class RespostaEadDto
    {
        public int Id { get; set; }
        public string TipoResposta { get; set; }
        public string Resposta { get; set; }
        public decimal ValorPesoResposta { get; set; }
        public bool RespostaCerta { get; set; }
    }
}
