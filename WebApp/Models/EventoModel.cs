using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class EventoModel
    {
        public string FomentoId { get; set; }
        public SelectList ListFomentos { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public string LocalidadeId { get; set; }
        public SelectList ListLocalidades { get; set; }
        public string AlunoId { get; set; }
        public SelectList ListAlunos { get; set; }
        public EventoDto Evento { get; set; }
        public PaginatedListDto<EventoDto>? Eventos { get; set; }
        public List<ControlePresencaDto> ControlesPresencas { get; set; }
        public AlunoIndexDto? Convidado { get; set; }
        public int IdPerfil { get; set; }
        public EventosFilterDto SearchFilter { get; set; }


        public class CreateUpdateEventoCommand
        {
            public int Id { get; set; }
            public int LocalidadeId { get; init; }
            public string? Titulo { get; init; }
            public string? Descricao { get; init; }
            public string? DataEvento { get; init; }
            public bool Status { get; set; } = true;

        }
    }

}
