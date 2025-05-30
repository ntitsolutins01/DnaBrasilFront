namespace WebApp.Dto
{
    public class MaterialDto
    {
        public required int Id { get; set; }
        public required int TipoMaterialId { get; set; }
        public required string TituloTipoMaterial { get; set; }
        public required string UnidadeMedida { get; set; }
        public required string Descricao { get; set; }
    }
}
