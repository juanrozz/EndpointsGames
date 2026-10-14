namespace EndpointsPage.DTOs.Request
{
    public class UserRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? DomainId {get; set; }
    }
}
