using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class PlanoAulaModel
    {
        public PlanoAulaDto PlanoAula { get; set; }
        public List<PlanoAulaDto> PlanosAulas { get; set; }
        public string PlanoAulaId { get; set; }
        public SelectList ListPlanoAulas { get; set; }
        
        public class CreateUpdatePlanoAulaCommand
        {

            public int Id { get; set; }
            public string? Nome { get; set; }
            public string? Grade { get; set; }
            public string? Url { get; set; }
        }
    }
}
