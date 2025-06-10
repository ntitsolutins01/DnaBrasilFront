using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class MaterialModel
    {
        public MaterialDto Material { get; set; }
        public List<MaterialIndexDto> Materiais { get; set; }
        public SelectList ListMateriais { get; set; }
        public List<GrupoMaterialDto> GruposMateriais { get; set; }
        public SelectList ListGruposMateriais { get; set; }
        public int GrupoMaterialId { get; set; }
        public TipoMaterialDto TipoMaterial { get; set; }
        public List<TipoMaterialDto> TiposMateriais { get; set; }
        public SelectList ListTiposMateriais { get; set; }
        public int TipoMaterialId { get; set; }
        public SelectList ListUnidadesMedidas { get; set; }
        public int UnidadeId { get; set; }
        public MateriaisFilterDto SearchFilter { get; set; }

        public class CreateUpdateMaterialCommand
        {
            public int Id { get; set; }
            public int TipoMaterialId { get; set; }
            public string UnidadeMedida { get; set; }
            public string Descricao { get; set; }
        }
    }

}