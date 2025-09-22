using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class AlternativasDto
    {
        public int EducacionalId { get; set; }
        public required string Alternativas { get; set; }
    }
}
