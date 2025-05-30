namespace WebApp.Dto
{
    public class ControlePresencaDto
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public int? EventoId { get; set; }
        public required string NomeAluno { get; set; }
        public required string Controle { get; set; }
        public string? Justificativa { get; set; }
        public string? MunicipioEstado { get; set; }
        public string? NomeLocalidade { get; set; }
        public string? Data { get; set; }
        public int? LocalidadeId { get; set; }
        public int? MunicipioId { get; set; }
        public bool Status { get; set; }
        public int Mes { get; internal set; }
    }
}
