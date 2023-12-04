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
        public string CidadeId { get; set; }
        public SelectList ListCidades { get; set; }

        public class CreateUpdateFomentoCommand
        {
            public string Id { get; set; }
            public string Nome { get; set; }
            public int MunicipioId { get; set; }
            public int LocalidadeId { get; set; }
        }
    }

}
