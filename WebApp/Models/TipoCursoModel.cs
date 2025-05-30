using WebApp.Dto;

namespace WebApp.Models
{
    public class TipoCursoModel
    {
        public TiposCursoDto TipoCurso { get; set; }
        public List<TiposCursoDto> TiposCursos { get; set; }
        public string TiposcursosId { get; set; }

        public class CreateUpdateTipoCursoCommand
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public bool Status { get; set; } = true;
        }
    }

}
