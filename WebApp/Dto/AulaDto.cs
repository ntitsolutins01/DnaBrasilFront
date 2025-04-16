using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class AulaDto
    {
		public required int Id { get; set; }
        public required int ModuloEadId { get; set; }
		public required string NomeProfessor { get; set; }
		public required int ProfessorId { get; set; }
		public required string TituloModuloEad { get; set; }
		public required string Titulo { get; set; }
		public string? Descricao { get; set; }
		public bool Status { get; set; }
        public SelectList? ListProfessores { get; set; }
        public string? Material { get; set; }
        public string? NomeMaterial { get; set; }
		public string? Video { get; set; }
		public string? NomeVideo { get; set; }
    }
}
