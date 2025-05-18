using WebApp.Dto;

namespace WebApp.Models
{
    public class GrupoMaterialModel
    {
        public GrupoMaterialDto GrupoMaterial { get; set; }
        public List<GrupoMaterialDto> GruposMateriais { get; set; }

        public class CreateUpdateGrupoMaterialCommand
        {
            public int Id { get; set; }
            public required string Nome { get; set; }
        }
    }

}