using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class QuestionarioModel
    {
        public List<QuestionarioDto> Questionarios { get; set; }
        public SelectList ListTiposLaudos { get; set; }
        public int TipoLaudoId { get; set; }

        public class CreateUpdateQuestionarioCommand
        {
            public int Id { get; set; }
            public string Pergunta { get; set; }
            public int TipoLaudoId { get; set; }
            public int Quadrante { get; set; }
            public int Questao { get; set; }
        }
    }

}