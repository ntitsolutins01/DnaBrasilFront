using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class FuncionalidadeModel
    {
        public FuncionalidadeDto Funcionalidade { get; set; }
        public List<FuncionalidadeDto> Funcionalidades { get; set; }
        public int FuncionalidadeId { get; set; }
        public SelectList ListFuncionalidades { get; set; }
        public int ModuloId { get; set; }
        public SelectList ListModulos { get; set; }

        public class CreateUpdateFuncionalidadeCommand
        {
            public int Id { get; set; }
            public int? ModuloId { get; set; }
            public string? Nome { get; set; }
        }
    }

}
