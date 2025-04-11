namespace WebApp.Dto
{
    public class InventarioDto
    {
        public required int Id { get; set; }
        public required int MaterialId { get; set; }
        public required string NomeMaterial { get; set; }
        public required int LocalidadeId { get; set; }
        public required string NomeLocalidade { get; set; }
        public int? Quantidade { get; set; }
        public required string Motivo { get; set; }
    }
}