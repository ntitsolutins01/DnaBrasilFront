using WebApp.Dto;

namespace WebApp.Models
{
    public class ControleAcessoAulaModel
    {

        public ControleAcessoAulaDto ControleAcessoAula { get; set; }
        public List<ControleAcessoAulaDto> ControlesAcessosAulas { get; set; }

        public class CreateUpdateControleAcessoAulaCommand
        {
            public int Id { get; set; }
            public required int AulaId { get; init; }
            public bool IdentificacaoAluno { get; init; }
            public bool AulaRequisito { get; init; }
            public bool PermanenciaAula { get; init; }
            public required string TempoPermanecia { get; init; }
            public required string LiberacaoAula { get; init; }
            public required string DataLiberacao { get; init; }
            public required string DataEncerramento { get; init; }
            public bool Status { get; set; }

        }
    }

}
