using EndpointsPage.DTOs.Response;
using EndpointsPage.Entities;

namespace EndpointsPage.Utilities
{
    public static class GlobalStatisticsMapper
    {
        public static StatsBestResponseDto GlobalStatisticsToDto(this MatchesJdr m)
        {
            if (m == null)
            {
                return null;
            }

            return new StatsBestResponseDto
            {
                Name = m.User.Name,
                Score = m.Score,
                Difficult = m.Level.LevelName,
                BestMatchDate = m.MatchDate
            };
        }
    }
}
