namespace WebApp.Dto
{
    public class AlunosFilterDto
    {

        #region SearchFilter
        public string? Sexo { get; set; }
        public string? FomentoId { get; set; }
        public string? Estado { get; set; }
        public string? MunicipioId { get; set; }
        public string? LocalidadeId { get; set; }
        public string? ProfissionalId { get; set; }
        public string? DeficienciaId { get; set; }
        public string? Etnia { get; set; }
        public string? Nome { get; set; }
        public string? Matricula { get; set; }
        public bool PossuiFoto { get; set; }
        public string? AlunoId { get; set; }


        #endregion

        public List<AlunoIndexDto>? Alunos { get; set; }
    }
}
