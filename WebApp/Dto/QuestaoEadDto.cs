namespace WebApp.Dto
{
    public class QuestaoEadDto
    {
        public int Id { get; set; }
        public required string NomeAula { get; set; }
        public string? Referencia { get; set; }
        public required string Enunciado { get; set; }
        public List<RespostaEadDto>? Respostas { get; set; }
        public required int NumeroQuestao { get; set; }
    }
}
