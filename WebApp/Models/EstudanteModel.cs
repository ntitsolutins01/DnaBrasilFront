using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class EstudanteModel
    {
        public EstudanteDto Estudante { get; set; }
        public List<EstudanteDto> Estudantes { get; set; }
        public string EstudanteId { get; set; }
        public SelectList ListEstudantes { get; set; }
        public SelectList ListLocalidades { get; set; }
        public SelectList ListDeficiencias { get; set; }
        public int LocalidadeId { get; set; }
        public int DeficienciaId { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public SelectList ListAmbientes { get; set; }
        public object AmbienteId { get; set; }
        public string ContratoId { get; set; }
        public string CategoriaId { get; set; }
        public SelectList ListContratos { get; set; }
        public SelectList ListCategorias { get; set; }

        public class CreateUpdateEstudanteCommand
        {
            public string Id { get; set; }
            public string Nome { get; set; }
        }
    }

}
