namespace WebApp.Dto
{
    public class AlunoIndexDto
    {
        public int Id { get; set; }
        public string AspNetUserId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string DtNascimento { get; set; }
        public bool Status { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public byte[] ByteImage { get; set; }
        public byte[] QrCode { get; set; }
        public string ModalidadeLinhaAcao { get; set; }

        public string MunicipioEstado { get; set; }
        public string NomeLocalidade { get; set; }

        public MunicipioDto Municipio { get; set; }
        public LocalidadeDto Localidade { get; set; }
        public bool Convidado { get; set; }
        public bool PossuiLaudo { get; set; }

        #region SearchFilter
        public string Sexo { get; set; }
        public string FomentoId { get; set; }
        public string Estado { get; set; }
        public string MunicipioId { get; set; }
        public string LocalidadeId { get; set; }
        public string DeficienciaId { get; set; }
        public string Etnia { get; set; }
        public string ProfissionalId { get; set; }
        #endregion
    }
}