namespace DemoApi.Models.Auth
{
    public class RefreshRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}