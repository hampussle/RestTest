using System.Threading.Tasks;
using DemoApi.Services;
using DemoApi.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController : Controller
    {
        private readonly IIdentityService _identityService;

        public UserController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            var response = await _identityService.RegisterAsync(request.Username, request.Password);
            if (!response.Success)
                return BadRequest(new RegistrationResponse() { Errors = response.Errors });
            return Ok(new RegistrationResponse() { Token = response.Token });
        }
    }

    public class UserRegistrationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}