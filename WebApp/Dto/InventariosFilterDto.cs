namespace WebApp.Dto
{
    public class InventariosFilterDto
    {
        #region SearchFilter
        public string? Id { get; set; }
        public string? NomeMaterial { get; set; }
        public string? MaterialId { get; set; }
        public string? LocalidadeId { get; set; }
        public string? TipoMaterialId { get; set; }
        public string? GrupoMaterialId { get; set; }

        #endregion

        public List<InventarioIndexDto> Inventarios { get; set; }
    }

}