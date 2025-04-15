using System.Collections;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using WebApp.Dto;

namespace WebApp.Models
{
	public partial class EstruturaCursoModel
	{
		public CursoDto Curso { get; set; }
		public List<CursoDto> Cursos { get; set; }
        public ModuloEadDto ModuloEad { get; set; }
        public List<ModuloEadDto> ModulosEad { get; set; }
        public AulaDto Aula { get; set; }
        public List<AulaDto> Aulas { get; set; }
        public SelectList ListTiposCursos { get; set; } 
        public string TipoCursoId { get; set; }
		public SelectList ListCoordenadores { get; set; }
        public string CoordenadorId { get; set; }
	}

}
