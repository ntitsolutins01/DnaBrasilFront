namespace WebApp.Dto
{
    public class TipoMaterialDto
    {
        public required int Id { get; set; }
        public required int GrupoMaterialId { get; set; }
        public required string TituloGrupoMaterial { get; set; }
        public required string Nome { get; set; }
    }
}
