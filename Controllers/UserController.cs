using System.Threading.Tasks;
using DemoApi.Models.Auth;
using DemoApi.Services;
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
        public async Task<IActionResult> RegisterAsync([FromBody] AuthenticationRequest request)
        {
            AuthenticationResult response = await _identityService.RegisterAsync(request.Username, request.Password);
            if (!response.Success)
                return BadRequest(new AuthenticationResult { Errors = response.Errors });
            return Ok(new AuthenticationResult { Token = response.Token, RefreshToken = response.RefreshToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] AuthenticationRequest request)
        {
            AuthenticationResult response = await _identityService.LoginAsync(request.Username, request.Password);
            if (!response.Success)
                return BadRequest(new AuthenticationResult { Errors = response.Errors });
            return Ok(new AuthenticationResult { Token = response.Token, RefreshToken = response.RefreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshRequest request)
        {
            AuthenticationResult response = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);
            if (!response.Success)
                return BadRequest(new AuthenticationResult { Errors = response.Errors });
            return Ok(new AuthenticationResult { Token = response.Token, RefreshToken = response.RefreshToken });
        }
    }
}