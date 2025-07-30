using WebApp.Dto;

namespace WebApp.Models
{
    public partial class EstruturaModuloEadModel
    {
        public CursoDto Curso { get; set; }
        public List<CursoDto> Cursos { get; set; }
        public ModuloEadDto ModuloEad { get; set; }
        public List<ModuloEadDto> ModulosEad { get; set; }
        public AulaDto Aula { get; set; }
        public List<AulaDto> Aulas { get; set; }
    }

}
