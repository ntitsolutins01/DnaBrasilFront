using System.Collections;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using WebApp.Dto;

namespace WebApp.Models
{
    public class ControleMaterialEstoqueSaidaModel
    {
        public ControleMaterialEstoqueSaidaDto ControleMaterialEstoqueSaida { get; set; }
        public List<ControleMaterialEstoqueSaidaDto> ControlesMateriaisEstoquesSaidas { get; set; }
        public SelectList ListControlesMateriaisEstoquesSaidas { get; set; }
        public TipoMaterialDto TipoMaterial { get; set; }
        public List<TipoMaterialDto> TiposMateriais { get; set; }
        public SelectList ListTiposMateriais { get; set; }
        public int TipoMaterialId { get; set; }
        public MaterialDto Material { get; set; }
        public List<MaterialDto> Materiais { get; set; }
        public SelectList ListMateriais { get; set; }
        public int MaterialId { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public SelectList ListMunicipios { get; set; }
        public string MunicipioId { get; set; }
        public LocalidadeDto Localidade { get; set; }
        public List<LocalidadeDto> Localidades { get; set; }
        public SelectList ListLocalidades { get; set; }
        public string LocalidadeId { get; set; }

        public class CreateUpdateControleMaterialEstoqueSaidaCommand
        {
            public int Id { get; set; }
            public int MunicipioId { get; set; }
            public int LocalidadeId { get; set; }
            public int MaterialId { get; set; }
            public  int Quantidade { get; set; }
            public string? Solicitante { get; set; }
        }
    }

}