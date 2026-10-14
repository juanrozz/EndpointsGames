using System.Text.RegularExpressions;

namespace EndpointsPage.DTOs.Request
{
    public class FinishMatchRequestDto
    {
        public int MatchId { get; set; }
        public int FinalScore { get; set; }
        public decimal FinalTime { get; set; } // in seconds
    }
}
