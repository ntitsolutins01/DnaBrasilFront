using System.ComponentModel.DataAnnotations;

namespace WebApp.Dto
{
    public class ControleMaterialEstoqueSaidaDto
    {
        public required int Id { get; set; }
        public required int MunicipioId { get; set; }
        public required int LocalidadeId { get; set; }
        public required int InventarioId { get; set; }
        public string? NomeMunicipio { get; set; }
        public string? NomeLocalidade { get; set; }
        public required string TituloMaterial { get; set; }
        public required int Quantidade { get; set; }
        public string? Solicitante { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? Created { get; set; }
    }
}
