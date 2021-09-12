using System.Threading.Tasks;
using DemoApi.Models.Auth;

namespace DemoApi.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string username, string password);
        Task<AuthenticationResult> LoginAsync(string username, string password);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    }
}