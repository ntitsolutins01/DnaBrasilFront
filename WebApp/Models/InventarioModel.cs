using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class InventarioModel
    {
        public InventarioDto Inventario { get; set; }
        public List<InventarioIndexDto> Inventarios { get; set; }
        public int InventarioId { get; set; }
        public SelectList ListInventarios { get; set; }
        public List<GrupoMaterialDto> GruposMateriais { get; set; }
        public SelectList ListGruposMateriais { get; set; }
        public int GrupoMaterialId { get; set; }
        public TipoMaterialDto TipoMaterial { get; set; }
        public List<TipoMaterialDto> TiposMateriais { get; set; }
        public SelectList ListTiposMateriais { get; set; }
        public int TipoMaterialId { get; set; }
        public MaterialDto Material { get; set; }
        public List<MaterialDto> Materiais { get; set; }
        public SelectList ListMateriais { get; set; }
        public int MaterialId { get; set; }
        public List<LocalidadeDto> Localidades { get; set; }
        public SelectList ListLocalidades { get; set; }
        public int LocalidadeId { get; set; }
        public List<ArquivosInventarioDto> ArquivosInventarios { get; set; }
        public SelectList ListArquivosInventarios { get; set; }
        public int ArquivosInventarioId { get; set; }
        public SelectList ListUnidadesMedidas { get; set; }
        public int UnidadeId { get; set; }
        public InventariosFilterDto SearchFilter { get; set; }

        public class CreateUpdateInventarioCommand
        {
            public int Id { get; set; }
            public int MaterialId { get; set; }
            public int LocalidadeId { get; set; }
            public int? Quantidade { get; set; }
            public string Motivo { get; set; }
        }
    }

}