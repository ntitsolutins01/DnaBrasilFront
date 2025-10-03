using System.ComponentModel;

namespace WebApp.Enumerators
{
    public enum EnumPerfil
    {
        [Description("Administrador")]
        Administrador = 4,
        [Description("Parceiro")]
        Parceiro = 8,
        [Description("Aluno")]
        Aluno = 6,
        [Description("Profissional")]
        Profissional = 7,
        [Description("Professor")]
        Professor = 13,
        [Description("Coordenador Pedagógico")]
        Coordenador = 14,
        [Description("Gestor Pedagógico")]
        GestorPedagogico = 9,
        [Description("Gestor do Projeto")]
        GestorProjeto = 10,
        [Description("Coordenador Ead")]
        CoordenadorEad = 15,
        [Description("Administrador Ead")]
        AdministradorEad = 17,
        [Description("Administrador Consulta")]
        AdministradorConsulta = 18,
        [Description("Coordenador Geral")]
        CoordenadorGeral = 19,
        [Description("Equipe Itinerante")]
        EquipeItinerante = 20
    }
}
