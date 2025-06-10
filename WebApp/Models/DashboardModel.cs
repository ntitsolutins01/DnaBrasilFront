using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class DashboardModel
    {
        public string FomentoId { get; set; }
        public SelectList ListFomentos { get; set; }
        public string LocalidadeId { get; set; }
        public SelectList ListLocalidades { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public DashboardDto Dashboard { get; set; }
        public SelectList ListDeficiencias { get; set; }
        public string DeficienciaId { get; set; }
        public SelectList ListEtnias { get; set; }
        public string EtniaId { get; set; }
        public DataGrafico DataGrafico { get; set; }
    }

    public class DataGrafico
    {
        public string name { get; set; }
        public decimal y { get; set; }
        public int z { get; set; }
    }

}
