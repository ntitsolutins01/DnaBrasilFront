using WebApp.Dto;

namespace WebApp.Models
{
    public class RankingModel
    {
        public RankingDto Ranking { get; set; }
        public List<RankingDto> MetricasImc { get; set; }

        public class CreateUpdateRankingCommand
        {
            public int Id { get; set; }
            public string? NomeRanking { get; set; }
            public bool Status { get; set; } = true;
        }
    }

}
