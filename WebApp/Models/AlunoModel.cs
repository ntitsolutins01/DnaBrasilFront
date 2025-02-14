using System.Collections.Specialized;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Data;
using WebApp.Dto;

namespace WebApp.Models
{
    public class AlunoModel
    {
        public AlunoDto Aluno { get; set; }
        public List<AlunoIndexDto>? Alunos { get; set; }
        public SelectList ListDeficiencias { get; set; }
        public string DeficienciaId { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public SelectList ListModalidades { get; set; }
        public int ModalidadeId { get; set; }
        public string FomentoId { get; set; }
        public SelectList ListFomentos { get; set; }
        public string? LocalidadeId { get; set; }
        public SelectList ListLocalidades { get; set; }
        public SelectList ListProfissionais { get; set; }
        public string ProfissionalId { get; set; }
        public List<ModalidadeDto>? Modalidades { get; set; }
        public SelectList ListEtnias { get; set; }
        public string EtniaId { get; set; }
        public SelectList ListSexos { get; set; }
        public string SexoId { get; set; }
        public string? NomePerfil { get; set; }
        public AlunosFilterDto SearchFilter { get; set; }
        public ModeloCarteirinhaDto ModeloCarteirinha { get; set; } 

        public class CreateUpdateDadosAlunoCommand
        {
            public int Id { get; set; }
            public string? AspNetUserId { get; set; }
            public int? MunicipioId { get; set; }
            public string? Nome { get; set; }
            public string? Email { get; set; }
            public string? Sexo { get; set; }
            public string? DtNascimento { get; set; }
            public string? NomeMae { get; set; }
            public string? NomePai { get; set; }
            public string? Cpf { get; set; }
            public string? Telefone { get; set; }
            public string? Celular { get; set; }
            public string? Cep { get; set; }
            public string? Endereco { get; set; }
            public string? Numero { get; set; }
            public string? Bairro { get; set; }
            public string? NomeFoto { get; set; }
            public bool Status { get; set; }
            public bool Habilitado { get; set; }
            public int? DeficienciaId { get; set; }
            public int? ProfissionalId { get; set; }
            public string? Etnia { get; set; }
            public string? ModalidadesIds { get; set; }
            public string? DeficienciasIds { get; set; }
            public int? LocalidadeId { get; set; }
            public string? NomeResponsavel { get; set; }
            public byte[]? ByteImage { get; set; }
            public byte[]? QrCode { get; set; }
            public bool? AutorizacaoSaida { get; set; } = false;
            public bool? AutorizacaoConsentimentoAssentimento { get; set; } = false;
            public bool? ParticipacaoProgramaCompartilhamentoDados { get; set; } = false;
            public bool? UtilizacaoImagem { get; set; } = false;
            public bool? CopiaDocAlunoResponsavel { get; set; } = false;
            public int? FomentoId { get; set; }
            public bool? Convidado { get; set; } = false;
        }
    }

}
