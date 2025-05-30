using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Dto
{
    public class ModalidadeDto
    {
        public int Id { get; init; }
        public string? Nome { get; init; }
        public int Vo2MaxIni { get; init; }
        public int Vo2MaxFim { get; init; }
        public int VinteMetrosIni { get; init; }
        public int VinteMetrosFim { get; init; }
        public int ShutlleRunIni { get; init; }
        public int ShutlleRunFim { get; init; }
        public int FlexibilidadeIni { get; init; }
        public int FlexibilidadeFim { get; init; }
        public int PreensaoManualIni { get; init; }
        public int PreensaoManualFim { get; init; }
        public int AbdominalPranchaIni { get; init; }
        public int AbdominalPranchaFim { get; init; }
        public int ImpulsaoIni { get; init; }
        public int ImpulsaoFim { get; init; }
        public int EnvergaduraIni { get; init; }
        public int EnvergaduraFim { get; init; }
        public int PesoIni { get; init; }
        public int PesoFim { get; init; }
        public int AlturaIni { get; init; }
        public int AlturaFim { get; init; }
        public bool Status { get; init; } = true;
        public byte[]? ByteImage { get; init; }
        public string? NomeByteImage { get; init; }
        public int? LinhaAcaoId { get; init; }
        public string? LinhaAcao { get; init; }
        public SelectList ListLinhasAcoes { get; set; }
    }
}
