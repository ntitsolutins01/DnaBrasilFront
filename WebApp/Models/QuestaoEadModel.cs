using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class QuestaoEadModel
    {
        public QuestaoEadDto QuestaoEad { get; set; }
        public List<QuestaoEadDto> QuestoesEad { get; set; }
        public SelectList ListQuestoesEad { get; set; }
        public RespostaEadDto RespostaEad { get; set; }
        public List<RespostaEadDto> RespostasEad { get; set; }
        public SelectList ListRespostasEad { get; set; }


        public string AulaId { get; set; }
        public SelectList ListAulas { get; set; }
        public string ModuloEadId { get; set; }
        public SelectList ListModulosEad { get; set; }
        public string CursoId { get; set; }
        public SelectList ListCursos { get; set; }
        public string TipoCursosId { get; set; }
        public SelectList ListTipoCursos { get; set; }



        public class CreateUpdateQuestaoEadCommand
        {
            public int Id { get; set; }
            public int AulaId { get; set; }
            public string Referencia { get; set; }
            public string Enunciado { get; set; }
            public List<RespostaEadDto>? Respostas { get; set; }
            public required int Questao { get; set; }
            public List<string?> ListTextos { get; set; }
            public List<string?> ListImagens { get; set; }
        }
    }

}