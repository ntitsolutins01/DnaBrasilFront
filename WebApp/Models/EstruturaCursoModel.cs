using WebApp.Dto;

namespace WebApp.Models
{
    public partial class EstruturaCursoModel
    {
        public CursoDto Curso { get; set; }
        public List<CursoDto> Cursos { get; set; }
        public ModuloEadDto ModuloEad { get; set; }
        public List<ModuloEadDto> ModulosEad { get; set; }
    }

}
