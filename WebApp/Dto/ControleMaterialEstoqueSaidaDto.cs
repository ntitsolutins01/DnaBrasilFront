using System.ComponentModel.DataAnnotations;

namespace WebApp.Dto
{
    public class ControleMaterialEstoqueSaidaDto
    {
        public required int Id { get; init; }
        public required int MunicipioId { get; init; }
        public required int LocalidadeId { get; init; }
        public required int MaterialId { get; init; }
        public string? NomeMunicipio { get; init; }
        public string? NomeLocalidade { get; init; }
        public required string TituloMaterial { get; init; }
        public required int Quantidade { get; init; }
        public string? Solicitante { get; init; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? Created { get; set; }
    }
}
