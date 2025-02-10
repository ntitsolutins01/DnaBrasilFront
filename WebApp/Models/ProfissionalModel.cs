using System.Collections;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class ProfissionalModel
    {
        public ProfissionalDto Profissional { get; set; }
        public List<ProfissionalDto> Profissionais { get; set; }
        public string ProfissionalId { get; set; }
        public SelectList ListProfissionals { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public SelectList ListModalidades { get; set; }
        public object ModalidadeId { get; set; }
        public List<ModalidadeDto> Modalidades { get; set; }
        public string LocalidadeId { get; set; }
        public SelectList ListLocalidades { get; set; }
        public int PerfilId { get; set; }
        public SelectList ListPerfis { get; set; }
        public SelectList ListCargos { get; set; }
        public string CargoId { get; set; }
        public UsuarioDto Usuario { get; set; }
        public int LinhaAcaoId { get; set; }
        public SelectList ListLinhasAcoes { get; set; }
        public int AtividadeModalidadeId { get; set; }
        public SelectList ListAtividadesModalidades { get; set; }
        public int AlunoId { get; set; }
        public SelectList ListAlunos { get; set; }

        public class CreateUpdateProfissionalCommand
        {
            public int Id { get; set; }
            public  string? AspNetUserId { get; set; }
            public  string? Nome { get; set; }
            public string? DtNascimento { get; set; }
            public string? Email { get; set; }
            public string? Sexo { get; set; }
            public  string? Cpf { get; set; }
            public string? Telefone { get; set; }
            public string? Celular { get; set; }
            public string? Endereco { get; set; }
            public int? Numero { get; set; }
            public string? Cep { get; set; }
            public string? Bairro { get; set; }
            public bool Status { get; set; } = true;
            public int? MunicipioId { get; set; }
            public int? LocalidadeId { get; set; }
            public bool Habilitado { get; set; }
            public string? ModalidadesIds { get; set; }
            public int? PerfilId { get; set; }
            public string? Cargo { get; set; }
        }
    }

}
