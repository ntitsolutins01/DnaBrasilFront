﻿namespace WebApp.Dto
{
	public class AlunosFilterDto
    {

		#region SearchFilter
		public string Sexo { get; set; }
		public string FomentoId { get; set; }
		public string Estado { get; set; }
		public string MunicipioId { get; set; }
		public string LocalidadeId { get; set; }
        public string DeficienciaId { get; set; }
        public string Etnia { get; set; }

        #endregion

        public List<AlunoDto>? Alunos { get; set; }
    }
}