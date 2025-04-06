using System.Collections;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using WebApp.Dto;

namespace WebApp.Models
{
	public class ModuloEadModel
	{
		public ModuloEadDto ModuloEad { get; set; }
		public List<ModuloEadDto> ModulosEad { get; set; }
		public SelectList ListModulosEad { get; set; } 
        public string ModuloEadId { get; set; }
        public string CursoId { get; set; }
        public string TipoCursosId { get; set; }
		public SelectList ListTipoCursos { get; set; }
		public SelectList ListCursos { get; set; }


        public class CreateUpdateModuloEadCommand
		{
			public int Id { get; set; }
			public int CursoId { get; set; }
			public required string Titulo { get; set; }
			public string? Descricao { get; set; }
			public bool Status { get; set; }
		}
	}

}
