namespace WebApp.Dto
{
    public class LaudosFilterDto
    {

        #region SearchFilter
        public required int PageNumber { get; set; } = 1;
        public required int PageSize { get; set; } = 10;
        public string FomentoId { get; set; }
        public string Estado { get; set; }
        public string MunicipioId { get; set; }
        public string LocalidadeId { get; set; }
        public string AlunoId { get; set; }
        public string TipoLaudoId { get; set; }
        public bool PossuiFoto { get; set; }
        public bool Finalizado { get; set; }
        public string DeficienciaId { get; set; }

        #endregion

        public PaginatedListDto<LaudoDto>? Laudos { get; set; }
        public string? UsuarioEmail { get; set; }
        public int? Ordem { get; set; }
    }
}
