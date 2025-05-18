using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class EncaminhamentoModel
    {
        public EncaminhamentoDto Encaminhamento { get; set; }
        public List<EncaminhamentoDto> Encaminhamentos { get; set; }
        public SelectList ListEstados { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListLocalidades { get; set; }
        public string LocalidadeId { get; set; }
        public SelectList ListAlunos { get; set; }
        public string AlunoId { get; set; }
        public SelectList ListDisciplinas { get; set; }
        public SelectList ListTiposLaudos { get; set; }
        public string DisciplinaId { get; set; }
        public string TipoLaudoId { get; set; }



        public class CreateUpdateEncaminhamentoCommand
        {
            public int Id { get; set; }
            public int TipoLaudoId { get; set; }
            public required string Nome { get; set; }
            public required string Parametro { get; set; }
            public string? Descricao { get; set; }
            public bool Status { get; set; } = true;
            public byte[]? ByteImage { get; set; }

        }
    }

}
