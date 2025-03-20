using System.Collections;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using WebApp.Dto;

namespace WebApp.Models
{
    public class ArquivosInventarioModel
    {
        public ArquivosInventarioDto ArquivosInventario { get; set; }
        public SelectList ListArquivosInventarios { get; set; }
        public InventarioDto Inventario { get; set; }
        public List<InventarioIndexDto> Inventarios { get; set; }
        public SelectList ListInventarios { get; set; }
        public List<GrupoMaterialDto> GruposMateriais { get; set; }
        public SelectList ListGruposMateriais { get; set; }
        public int GrupoMaterialId { get; set; }
        public TipoMaterialDto TipoMaterial { get; set; }
        public List<TipoMaterialDto> TiposMateriais { get; set; }
        public SelectList ListTiposMateriais { get; set; }
        public int TipoMaterialId { get; set; }
        public List<LocalidadeDto> Localidades { get; set; }
        public SelectList ListLocalidades { get; set; }
        public int LocalidadeId { get; set; }
        public InventariosFilterDto SearchFilter { get; set; }

        public class CreateUpdateArquivosInventarioCommand
        {
            public int Id { get; set; }
            public int InventarioId { get; set; }
            public string? PathArquivo { get; set; }
            public string? NomeArquivo { get; set; }
        }
    }

}