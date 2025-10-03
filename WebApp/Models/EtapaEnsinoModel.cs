using WebApp.Dto;

namespace WebApp.Models
{
  public class EtapaEnsinoModel
  {
    public EtapaEnsinoDto EtapaEnsino { get; set; } = new EtapaEnsinoDto();
    public List<EtapaEnsinoDto> EtapasEnsino { get; set; } = new List<EtapaEnsinoDto>();
    public int EtapaEnsinoId { get; set; }
    public string? Nome { get; set; }
    public bool? Status { get; set; }

    public class CreateEtapaEnsinoCommand
    {
      public required string Nome { get; set; }
      public bool Status { get; set; } = true;
    }

    public class UpdateEtapaEnsinoCommand
    {
      public int Id { get; set; }
      public required string Nome { get; set; }
      public required bool Status { get; set; }
    }

    public class DeleteEtapaEnsinoCommand
    {
      public int Id { get; set; }
    }
  }
}