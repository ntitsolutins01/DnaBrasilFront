using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class ControleFrequenciaEscolarModel
    {
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public string LocalidadeId { get; set; }
        public SelectList ListLocalidades { get; set; }
        public string TurmaId { get; set; }
        public SelectList ListTurmas { get; set; }
        public string ProfissionalId { get; set; }
        public SelectList ListProfissionais { get; set; }
        public string DisciplinaId { get; set; }
        public SelectList ListDisciplinas { get; set; }
        public string SerieId { get; set; }
        public SelectList ListSeries { get; set; }
        public string Data { get; set; }

        public class CreateUpdateControleFrequenciaEscolarCommand
        {
            public int Id { get; set; }
            public string Controle { get; init; }
            public string Justificativa { get; init; }
            public bool Status { get; init; } = true;
            public int? LocalidadeId { get; set; }
            public string? MunicipioId { get; set; }
            public string? AlunoId { get; set; }
            public int? EventoId { get; set; }
        }
    }

}
