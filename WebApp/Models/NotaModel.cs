using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class NotaModel
    {
        public NotaDto Nota { get; set; }
        public List<NotaDto> Notas { get; set; }
        public SelectList ListEstados { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListLocalidades { get; set; }
        public string LocalidadeId { get; set; }
        public SelectList ListAlunos { get; set; }
        public string AlunoId { get; set; }
        public SelectList ListDisciplinas { get; set; }
        public string DisciplinaId { get; set; }



        public class CreateUpdateNotaCommand
        {
            public int Id { get; set; }
            public string? AlunoId { get; set; }
            public string? DisciplinaId { get; set; }
            public decimal? PrimeiroBimestre { get; set; }

            [Column(TypeName = "decimal(10,2)")]
            public decimal? SegundoBimestre { get; set; }

            [Column(TypeName = "decimal(10,2)")]
            public decimal? TerceiroBimestre { get; set; }

            [Column(TypeName = "decimal(10,2)")]
            public decimal? QuartoBimestre { get; set; }

            [Column(TypeName = "decimal(10,2)")]
            public bool Status { get; set; } = true;

        }
    }

}
