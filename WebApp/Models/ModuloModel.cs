using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class ModuloModel
    {
        public ModuloDto Modulo { get; set; }
        public List<ModuloDto> Modulos { get; set; }
        public string ModuloId { get; set; }
        public SelectList ListModulos { get; set; }

        public class CreateUpdateModuloCommand
        {
            public int Id { get; set; }
            public string Nome { get; set; }

        }
    }

}