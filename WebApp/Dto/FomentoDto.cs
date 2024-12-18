﻿namespace WebApp.Dto
{
    public class FomentoDto
    {
        public string Id { get; set; }
        public string IdIdMunicipio { get; set; }
        public string Nome { get; set; }
        public bool Status { get; set; }
		public int MunicipioId { get; set; }
        public int LocalidadeId { get; set; }
        public string Data { get; set; }
        public string Codigo { get; set; }
        public string DtIni { get; set; }
        public string DtFim { get; set; }
        public string? MunicipioEstado { get; init; }
        public string? Localidade { get; init; }
    }
}
