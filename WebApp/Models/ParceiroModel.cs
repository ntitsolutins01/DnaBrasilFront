using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class ParceiroModel
    {
        public ParceiroDto Parceiro { get; set; }
        public List<ParceiroDto> Parceiros { get; set; }
        public string ParceiroId { get; set; }
        public SelectList ListParceiros { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public SelectList ListTiposParcerias { get; set; }
        public int TipoParceriaId { get; set; }
        public List<AlunoIndexDto> Alunos { get; set; }

        public class CreateUpdateParceiroCommand
        {
            public int Id { get; set; }
            public string? AspNetUserId { get; set; }
            public int? MunicipioId { get; set; }
            public string? Nome { get; set; }
            public string? Email { get; set; }
            public int? TipoParceriaId { get; set; }
            public string? TipoPessoa { get; set; }
            public string? CpfCnpj { get; set; }
            public string? Telefone { get; set; }
            public string? Celular { get; set; }
            public string? Cep { get; set; }
            public string? Endereco { get; set; }
            public int? Numero { get; set; }
            public string? Bairro { get; set; }
            public bool Status { get; set; } = true;
            public bool Habilitado { get; set; }
            public string RazaoSocial { get; set; }
            public string NomeContato { get; set; }
        }
    }

}
