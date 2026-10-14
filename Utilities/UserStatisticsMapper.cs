using EndpointsPage.DTOs.Response;
using EndpointsPage.Entities;

namespace EndpointsPage.Utilities
{
    public static class UserStatisticsMapper
    {
        public static UserStatisticsResponseDto MatchesJdrsToDto(this MatchesJdr bestMatch, int totalScore)
        {
            if (bestMatch == null)
            {
                return null;
            }

            return new UserStatisticsResponseDto
            {
                Name = bestMatch.User.Name,
                Difficult = bestMatch.Level.LevelName,
                TotalScore = totalScore,
                BestScore = bestMatch.Score,
                BestMatchDate = bestMatch.MatchDate
            };
        }
    }
}
