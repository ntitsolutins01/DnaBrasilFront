using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class DashboardEadModel
    {
        public string FomentoId { get; set; }
        public SelectList ListFomentos { get; set; }
        public string LocalidadeId { get; set; }
        public SelectList ListLocalidades { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public string CursoId { get; set; }
        public SelectList ListCursos { get; set; }
        public string TipoCursosId { get; set; }
        public SelectList ListTipoCursos { get; set; }
        public DashboardEadDto DashboardEad { get; set; }
    }


}
