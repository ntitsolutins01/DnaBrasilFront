using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class TipoMaterialModel
    {
        public TipoMaterialDto TipoMaterial { get; set; }
        public List<TipoMaterialDto> TiposMateriais { get; set; }
        public SelectList ListTiposMateriais { get; set; }
        public int GrupoMaterialId { get; set; }
        public TipoMaterialDto GrupoMaterial { get; set; }
        public List<TipoMaterialDto> GruposMateriais { get; set; }
        public SelectList ListGruposMateriais { get; set; }

        public class CreateUpdateTipoMaterialCommand
        {
            public int Id { get; set; }
            public int GrupoMaterialId { get; set; }
            public required string Nome { get; set; }
        }
    }

}