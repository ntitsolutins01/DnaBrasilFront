namespace WebApp.Dto
{
    public class RespostaDto
    {
        public int Id { get; init; }
        public int QuestionarioId { get; init; }
        public required string Pergunta { get; set; }
        public string? Descricao { get; set; }
        public int TipoLaudoId { get; init; }
        public required string NomeTipoLaudo { get; set; }
        public required string RespostaQuestionario { get; set; }
        public required decimal ValorPesoResposta { get; set; }
    }
}
