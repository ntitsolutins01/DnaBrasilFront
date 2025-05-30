namespace WebApp.Dto
{
    public class ContratoDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DtIni { get; set; }
        public DateTime DtFim { get; set; }
        public bool Status { get; set; } = true;
        public string Anexo { get; set; }

    }
}
