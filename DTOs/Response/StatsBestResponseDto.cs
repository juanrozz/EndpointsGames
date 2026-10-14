namespace EndpointsPage.DTOs.Response
{
    public class StatsBestResponseDto
    {
        public string? Name { get; set; }
        public int Score { get; set; }
        public string? Difficult { get; set; }
        public DateTime? BestMatchDate { get; set; }
    }
}
