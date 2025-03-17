namespace WebApp.Dto
{
    public class InventarioIndexDto
    {
        public required int Id { get; set; }
        public required string NomeMaterial { get; set; }
        public required string NomeLocalidade { get; set; }
        public required string NomeUndMedida { get; set; }
        public int? Quantidade { get; set; }

        #region SearchFilter
        public string? MaterialId { get; set; }
        public string? LocalidadeId { get; set; }
        public string? TipoMaterialId { get; set; }
        public string? GrupoMaterialId { get; set; }

        #endregion
    }
}