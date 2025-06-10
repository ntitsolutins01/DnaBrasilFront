using WebApp.Dto;

namespace WebApp.Models
{
    public class CategoriaModel
    {
        public CategoriaDto Categoria { get; set; }
        public List<CategoriaDto> Categorias { get; set; }

        public class CreateUpdateCategoriaCommand
        {
            public int Id { get; set; }
            public required string Codigo { get; set; }
            public required string Nome { get; set; }
            public required int IdadeInicial { get; set; }
            public required int IdadeFinal { get; set; }
            public string? Descricao { get; set; }
            public bool Status { get; set; }
        }
    }

}
