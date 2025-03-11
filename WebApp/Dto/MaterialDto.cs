namespace WebApp.Dto
{
    public class MaterialDto
    {
        public required int Id { get; set; }
        public int? LocalidadeId { get; set; }
        public required int TipoMaterialId { get; set; }
        public string? NomeLocalidade { get; set; }
        public required string TituloTipoMaterial { get; set; }
        public required string UnidadeMedida { get; set; }
        public required string Descricao { get; set; }
        public int? QtdAdquirida { get; set; }
    }
}
