using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class MetricaImcModel
    {
        public MetricaImcDto MetricaImc { get; set; }
        public List<MetricaImcDto> MetricasImc { get; set; }
        public string MetricaImcId { get; set; }
        public SelectList ListMetricasImc { get; set; }

        public class CreateUpdateMetricaImcCommand
        {
            public int Id { get; set; }
            public string? Sexo { get; set; }
            public int? Idade { get; set; }
            public string? Classificacao { get; set; }
            public decimal ValorInicial { get; set; }
            public decimal ValorFinal { get; set; }
            public bool Status { get; set; } = true;
        }
    }

}
