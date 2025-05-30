namespace WebApp.Dto
{
    public class MateriaisFilterDto
    {

        #region SearchFilter
        public string? Id { get; set; }
        public string? LocalidadeId { get; set; }
        public string? NomeMaterial { get; set; }
        public string? GrupoMaterialId { get; set; }
        public string? TipoMaterialId { get; set; }

        #endregion

        public List<MaterialIndexDto> Materiais { get; set; }
    }
}