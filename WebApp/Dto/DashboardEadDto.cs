using WebApp.Models;

namespace WebApp.Dto
{
	public class DashboardEadDto
	{
        public int CursosDisponiveis { get; set; }
        public int CursosEmAndamento { get; set; }
        public int CursosFinalizados { get; set; }
        public int CadastrosMasculinos { get; set; }
        public int CadastrosFemininos { get; set; }
        public int AlunosCadastrados { get; set; }

        #region SearchFilter
        public string Sexo { get; set; }
		public string FomentoId { get; set; }
		public string Estado { get; set; }
		public string MunicipioId { get; set; }
		public string LocalidadeId { get; set; }
        public string TipoCursoId { get; set; }
        public string CursoId { get; set; }

        #endregion

    }
}
