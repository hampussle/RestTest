using System.Threading.Tasks;
using DemoApi.Routing.v1.DataTransferObjects;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto request)
        {
            var response = await _identityService.RegisterAsync(request.Username, request.Password);
            if (!response.Success)
                return BadRequest(new RegistrationResponse() { Errors = response.Errors });
            return Ok(new RegistrationResponse() { Token = response.Token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto request)
        {
            var response = await _identityService.LoginAsync(request.Username, request.Password);
            if (!response.Success)
                return BadRequest(new RegistrationResponse() { Errors = response.Errors });
            return Ok(new RegistrationResponse() { Token = response.Token });
        }
    }
}