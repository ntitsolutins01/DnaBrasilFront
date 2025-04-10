namespace WebApp.Dto
{
    public class EncaminhamentoDto
    {
        public required int Id { get; set; }
        public required string Nome { get; set; }
        public required string NomeTipoLaudo { get; set; }
        public required string Parametro { get; set; }
        public string? Descricao { get; set; }
        public bool Status { get; set; }
        public byte[]? ByteImage { get; set; }
        public string? EncaminhamentoTexto { get; set; }
    }
}
