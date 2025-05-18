using WebApp.Dto;

namespace WebApp.Models
{
    public class DependenciaModel
    {
        public DependenciaDto Dependencia { get; set; }
        public List<DependenciaDto> Dependencias { get; set; }

        public class CreateUpdateDependenciaCommand
        {
            public int Id { get; set; }
            public string? Doencas { get; set; }
            public string? Nacionalidade { get; set; }
            public string? Naturalidade { get; set; }
            public string? NomeEscola { get; set; }
            public string? TipoEscola { get; set; }
            public string? TipoEscolaridade { get; set; }
            public string? Turno { get; set; }
            public string? Serie { get; set; }
            public string? Ano { get; set; }
            public string? Turma { get; set; }
            public bool? TermoCompromisso { get; set; }
            public bool? AutorizacaoUsoImagemAudio { get; set; }
            public bool? AutorizacaoUsoIndicadores { get; set; }
            public bool? AutorizacaoSaida { get; set; } = false;
            public int? AlunoId { get; set; }
        }
    }

}
