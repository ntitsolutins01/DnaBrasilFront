namespace WebApp.Dto
{
    public class LocalidadeDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Status { get; set; }
        public int MunicipioId { get; set; }
        public int EstadoId { get; set; }
        public string? NomeMunicipio { get; set; }
        public string? NomeEstado { get; set; }
        public int? CodigoInep { get; set; }
    }
}
