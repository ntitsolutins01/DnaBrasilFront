namespace WebApp.Dto
{
    public class MaterialIndexDto
    {
        public required int Id { get; set; }
        public required string NomeLocalidade { get; set; }
        public required string TituloTipoMaterial { get; set; }
        public required string UnidadeMedida { get; set; }
        public required string Descricao { get; set; }
        public int? QtdAdquirida { get; set; }

        #region SearchFilter
        public string? MaterialId { get; set; }
        public string? NomeMaterial { get; set; }
        public string? GrupoMaterialId { get; set; }
        public string? TipoMaterialId { get; set; }
        #endregion
    }
}