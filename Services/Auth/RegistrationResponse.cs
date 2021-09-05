using System.Collections.Generic;

namespace DemoApi.Services.Auth
{
    public class RegistrationResponse
    {
        public string Token { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}