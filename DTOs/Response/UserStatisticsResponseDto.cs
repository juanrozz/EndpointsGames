namespace EndpointsPage.DTOs.Response
{
    public class UserStatisticsResponseDto
    {
        public string? Name { get; set; }
        public string? Difficult { get; set; }
        public int TotalScore { get; set; }
        public int BestScore { get; set; }
        public DateTime? BestMatchDate { get; set; }
    }
}
