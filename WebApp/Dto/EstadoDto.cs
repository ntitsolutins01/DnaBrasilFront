namespace WebApp.Dto
{
    public class EstadoDto
    {
        public int Id { get; init; }
        public string? Sigla { get; init; }
        public string? Nome { get; init; }
        public List<MunicipioDto>? Municipios { get; set; }

    }
}
