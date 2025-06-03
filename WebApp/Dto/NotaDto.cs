using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Dto
{
    public class NotaDto
    {
        public int Id { get; set; }
        public AlunoDto? Aluno { get; set; }
        public DisciplinaDto? Disciplina { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? PrimeiroBimestre { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SegundoBimestre { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? TerceiroBimestre { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? QuartoBimestre { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Media { get; set; }
        public string? LocalidadeMunicipioUf { get; set; }
        public bool Status { get; set; } = true;
    }
}
