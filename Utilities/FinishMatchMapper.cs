using EndpointsPage.DTOs.Response;
using EndpointsPage.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EndpointsPage.Utilities
{
    public static class FinishMatchMapper
    {
        public static FinishMatchResponseDto FinishMatchesJdrToDto(this MatchesJdr finishMatch)
        {
            if (finishMatch == null)
            {
                return null;
            }
            if (finishMatch.GameId == 1)
            {
                return new FinishMatchResponseDto
                {
                    FinalScore = finishMatch.Score,
                    FinalTimeSeconds = finishMatch.TimeSeconds
                };
            }
            else
            {
                return new FinishMatchResponseDto
                {
                    FinalScore = finishMatch.Score
                };
            }
        }
    }
}
