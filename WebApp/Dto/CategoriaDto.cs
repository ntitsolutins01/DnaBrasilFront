namespace WebApp.Dto
{
    public class CategoriaDto
    {
        public required int Id { get; set; }
        public required string Codigo { get; set; }
        public required string Nome { get; set; }
        public required int IdadeInicial { get; set; }
        public required int IdadeFinal { get; set; }
        public string? Descricao { get; set; }
        public bool Status { get; set; }
    }
}
