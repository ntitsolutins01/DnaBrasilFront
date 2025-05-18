using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class CursoDto
    {
        public required int Id { get; init; }
        public required int TipoCursoId { get; init; }
        public required string TituloTipoCurso { get; init; }
        public required int CoordenadorId { get; init; }
        public required string NomeCoordenador { get; init; }
        public required string Titulo { get; init; }
        public required int CargaHoraria { get; init; }
        public string? Descricao { get; init; }
        public bool Status { get; init; }
        public SelectList? ListCoordenadores { get; set; }
        public string? Imagem { get; set; }
        public string? NomeImagem { get; set; }
    }
}
