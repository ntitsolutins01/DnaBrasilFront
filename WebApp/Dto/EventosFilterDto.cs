namespace WebApp.Dto
{
    public class EventosFilterDto
    {

        #region SearchFilter
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string FomentoId { get; set; }
        public string Estado { get; set; }
        public string MunicipioId { get; set; }
        public string LocalidadeId { get; set; }

        #endregion

        public PaginatedListDto<EventoDto>? Eventos { get; set; }
    }
}
