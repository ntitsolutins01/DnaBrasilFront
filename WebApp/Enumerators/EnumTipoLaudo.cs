using System.ComponentModel;

namespace WebApp.Enumerators
{
    public enum EnumTipoLaudo
    {
        [Description("Talento Esportivo")]
        TalentoEsportivo = 4,
        [Description("Saúde Bucal")]
        SaudeBucal = 5,
        [Description("Vocacional")]
        Vocacional = 6,
        [Description("Qualidade de Vida")]
        QualidadeVida = 7,
        [Description("Consumo Alimentar")]
        ConsumoAlimentar = 8,
        [Description("Saúde")]
        Saude = 9,
        [Description("3LP")]
        Educacional3LP = 10,
        [Description("3MT")]
        Educacional3MT = 11,
        [Description("5LP")]
        Educacional5LT = 12,
        [Description("5MT")]
        Educacional5MT = 13,
        [Description("9LP")]
        Educacional9LP = 14,
        [Description("9MT")]
        Educacional9MT = 15
    }
}
