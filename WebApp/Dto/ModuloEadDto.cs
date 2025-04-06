namespace WebApp.Dto
{
    public class ModuloEadDto
    {
		public required int Id { get; set; }
		public required string Titulo { get; set; }
		public string? Descricao { get; set; }
		public bool Status { get; set; }
		public required int CursoId { get; set; }
		public required string TituloCurso { get; set; }
	}
}
