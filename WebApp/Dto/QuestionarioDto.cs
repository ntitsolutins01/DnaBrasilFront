namespace WebApp.Dto
{
    public class QuestionarioDto
    {
        public int Id { get; set; }
        public string Pergunta { get; set; }
        public TiposLaudoDto TipoLaudo { get; set; }
        public List<RespostaDto> Respostas { get; set; }
        public int Quadrante { get; set; }
        public int Questao { get; set; }
    }
}
