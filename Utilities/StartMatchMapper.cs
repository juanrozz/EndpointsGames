using EndpointsPage.DTOs.Request;
using EndpointsPage.DTOs.Response;
using EndpointsPage.Entities;

namespace EndpointsPage.Utilities
{
    public static class StartMatchMapper
    {
        public static MatchesJdr DtoToMatchesJdr(this StartMatchRequestDto request, int tokenUserId)
        {
            if (request == null)
            {
                return null;
            }

            return new MatchesJdr
            {
                UserId = tokenUserId,
                GameId = request.GameId,
                LevelId = request.Difficult,
                Score = 0,
                TimeSeconds = 0,
                MatchDate = DateTime.Now
            };
        
        }

        public static StartMatchResponseDto StartMatchesJdrToDto(this MatchesJdr match)
        {
            if(match == null)
            {
                return null;
            }

            return new StartMatchResponseDto
            {
                MatchId = match.MatchId
            };
        }

    }
}
