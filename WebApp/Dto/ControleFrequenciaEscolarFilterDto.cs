namespace WebApp.Dto
{
    public class ControleFrequenciaEscolarFilterDto
    {

        #region SearchFilter
        public string? Estado { get; set; }
        public string? MunicipioId { get; set; }
        public string? LocalidadeId { get; set; }
        public string? ProfissionalId { get; set; }
        public string? Nome { get; set; }
        public string? SerieId { get; set; }
        public string? AlunoId { get; set; }


        #endregion

        public List<ControleFrequenciaEscolarDto>? FrequenciasEscolares { get; set; }
    }
}
