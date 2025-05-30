namespace WebApp.Dto
{
    public class AtividadeDto
    {
        public required int Id { get; set; }
        public required int EstruturaId { get; set; }
        public required string NomeEstrutura { get; set; }
        public required int LinhaAcaoId { get; set; }
        public required string NomeLinhaAcao { get; set; }
        public required int CategoriaId { get; set; }
        public required string NomeCategoria { get; set; }
        public required int ModalidadeId { get; set; }
        public required string NomeModalidade { get; set; }
        public required string Turma { get; set; }
        public required string DiasSemana { get; set; }
        public required string HrInicial { get; set; }
        public required string HrFinal { get; set; }
        public required int ProfissionalId { get; set; }
        public required string NomeProfissional { get; set; }
        public required int LocalidadeId { get; set; }
        public required string NomeLocalidade { get; set; }
        public bool Status { get; set; }
        public required string TurmaHora { get; init; }
    }
}
