using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class DisciplinaModel
    {
        public DisciplinaDto Disciplina { get; set; }
        public List<DisciplinaDto> Disciplinas { get; set; }
        public string DisciplinaId { get; set; }
        public SelectList ListDisciplinas { get; set; }

        public class CreateUpdateDisciplinaCommand
        {
            public int Id { get; set; }
            public string Nome { get; set; }

        }
    }

}