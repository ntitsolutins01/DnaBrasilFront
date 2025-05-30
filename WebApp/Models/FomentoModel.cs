using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class FomentoModel
    {
        public string LocalidadeId { get; set; }
        public SelectList ListLocalidades { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public List<FomentoDto> Fomentos { get; set; }
        public string LinhaAcaoId { get; set; }
        public SelectList ListLinhasAcoes { get; set; }
        public FomentoDto Fomento { get; set; }

        public class CreateUpdateFomentoCommand
        {
            public int Id { get; set; }
            public string Codigo { get; set; }
            public string Nome { get; set; }
            public bool Status { get; set; }
            public string DtIni { get; set; }
            public string DtFim { get; set; }
            public int MunicipioId { get; set; }
            public int LocalidadeId { get; set; }
            public string LinhasAcoesIds { get; set; }
            public string LocalidadesIds { get; set; }
        }
    }

}
