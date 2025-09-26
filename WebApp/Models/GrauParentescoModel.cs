using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class GrauParentescoModel
    {
        public GrauParentescoDto GrauParentesco { get; set; }
        public List<GrauParentescoDto> GrauParentescos { get; set; }

        public class CreateUpdateGrauParentescoCommand
        {
            public int Id { get; set; }
            public string Nome { get; set; }

        }
    }

}