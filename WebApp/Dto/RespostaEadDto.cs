namespace WebApp.Dto
{
    public class RespostaEadDto
    {
        public int Id { get; init; }
        public required QuestaoEadDto Questao { get; init; }
        public required string TipoResposta { get; init; }
        public string? TipoAlternativa { get; init; }
        public required string Resposta { get; init; }
        public required decimal ValorPesoResposta { get; init; }
    }
}
