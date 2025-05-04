namespace WebApp.Dto
{
	public class LaudosResumidosFilterDto
    {

        #region SearchFilter
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

        public List<LaudoResumidoDto>? LaudosResumidos { get; set; }
        public string? UsuarioEmail { get; set; }
    }
}
