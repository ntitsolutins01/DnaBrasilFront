using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class RespostaModel
    {
        public RespostaDto Resposta { get; set; }
        public List<RespostaDto> Respostas { get; set; }
        public string QuestionarioId { get; set; }
        public SelectList ListQuestionarios { get; set; }
        public List<QuestionarioDto> Questionarios { get; set; }
        public int TipoLaudoId { get; set; }
        public SelectList ListTiposLaudos { get; set; }

        public class CreateUpdateRespostaCommand
        {
            public int Id { get; set; }
            public string RespostaQuestionario { get; set; }
            public int QuestionarioId { get; set; }
            public decimal? ValorPesoResposta { get; set; }
            public string? Descricao { get; set; }
        }
    }

}