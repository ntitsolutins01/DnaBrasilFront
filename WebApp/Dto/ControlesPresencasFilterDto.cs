namespace WebApp.Dto
{
    public class ControlesPresencasFilterDto
    {

        #region SearchFilter
        public required int PageNumber { get; set; } = 1;
        public required int PageSize { get; set; } = 10;
        public string Sexo { get; set; }
        public string FomentoId { get; set; }
        public string Estado { get; set; }
        public string MunicipioId { get; set; }
        public string LocalidadeId { get; set; }
        public string DeficienciaId { get; set; }
        public string Etnia { get; set; }
        public string UsuarioEmail { get; set; }

        #endregion

        public PaginatedListDto<ControlePresencaDto>? ControlesPresencas { get; set; }
    }
}
