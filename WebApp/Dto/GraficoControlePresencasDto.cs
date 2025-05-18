namespace WebApp.Dto
{
    public class GraficoControlePresencasDto
    {
        public int[]? ListPresencasAnual { get; set; }
        public int[]? ListFaltasAnual { get; set; }


        #region SearchFilter
        public string? FomentoId { get; set; }
        public string? Estado { get; set; }
        public string? MunicipioId { get; set; }
        public string? LocalidadeId { get; set; }
        public string? DeficienciaId { get; set; }
        public string? Etnia { get; set; }
        public string? Controle { get; set; }
        #endregion
    }
}
