namespace EndpointsPage.DTOs.Response
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public int IdentifierUser { get; set; }
        public string Username { get; set; }
        public bool? IsActive { get; set; }
        public int? UserLevel { get; set; }

    }
}
