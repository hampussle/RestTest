using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DemoApi.Data.Repositories;
using DemoApi.Models.Auth;
using DemoApi.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DemoApi.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenValidationParameters _tokenValidation;
        private readonly IAsyncRepository<RefreshToken> _tokenRepo;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidation, IAsyncRepository<RefreshToken> tokenRepo)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidation = tokenValidation;
            _tokenRepo = tokenRepo;
        }

        public async Task<AuthenticationResult> RegisterAsync(string username, string password)
        {
            IdentityUser user = await _userManager.FindByNameAsync(username);
            if (user != null) return new AuthenticationResult { Errors = new[] { "Username is taken" } };

            IdentityUser newUser = new() { UserName = username };

            IdentityResult createdUser = await _userManager.CreateAsync(newUser, password);

            return createdUser.Succeeded
                ? await JwtAuthAsync(newUser)
                : new AuthenticationResult { Errors = createdUser.Errors.Select(x => x.Description) };
        }

        public async Task<AuthenticationResult> LoginAsync(string username, string password)
        {
            IdentityUser user = await _userManager.FindByNameAsync(username);
            if (user is null) return new AuthenticationResult { Errors = new[] { "User does not exist" } };

            bool validPassword = await _userManager.CheckPasswordAsync(user, password);

            return validPassword
                ? await JwtAuthAsync(user)
                : new AuthenticationResult { Errors = new[] { "Incorrect username & password" } };
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validToken = GetPrincipal(token);
            if (validToken is null)
                return new AuthenticationResult { Errors = new[] { "Invalid token" } };

            string GetClaim(string claimType) => validToken.Claims.Single(x => x.Type == claimType).Value;

            long expiryDate = long.Parse(GetClaim(JwtRegisteredClaimNames.Exp));
            var expiryDateTime = DateTime.UnixEpoch.AddSeconds(expiryDate);
            if (expiryDateTime > DateTime.UtcNow)
                return new AuthenticationResult { Errors = new[] { "Token is not expired" } };
            string jti = GetClaim(JwtRegisteredClaimNames.Jti);
            var storedRefreshToken = await _tokenRepo.SingleOrDefaultAsync(x => x.Token.ToString() == refreshToken);
            if (storedRefreshToken is null)
                return new AuthenticationResult { Errors = new[] { "Refresh token does not exist" } };
            if (DateTime.UtcNow > storedRefreshToken.ExpirationDate)
                return new AuthenticationResult { Errors = new[] { "Refresh token is expired" } };
            if (storedRefreshToken.Invalidated)
                return new AuthenticationResult { Errors = new[] { "Refresh token has been destroyed" } };
            if (storedRefreshToken.Used)
                return new AuthenticationResult { Errors = new[] { "Refresh token has been used" } };
            if (storedRefreshToken.JwtId != jti)
                return new AuthenticationResult { Errors = new[] { "Refresh token does not match your user" } };

            storedRefreshToken.Used = true;
            _tokenRepo.Update(storedRefreshToken);
            await _tokenRepo.SaveAsync();

            var user = await _userManager.FindByIdAsync(GetClaim("id"));
            return await JwtAuthAsync(user);
        }

        private ClaimsPrincipal GetPrincipal(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            var principal = tokenHandler.ValidateToken(token, _tokenValidation, out SecurityToken validToken);
            return HasCorrectAlg(validToken) ? principal : null;
        }

        private static bool HasCorrectAlg(SecurityToken validToken)
        {
            return (validToken is JwtSecurityToken token) &&
                   token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> JwtAuthAsync(IdentityUser user)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim("id", user.Id)
                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            RefreshToken refreshToken = new()
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMonths(3)
            };

            await _tokenRepo.AddAsync(refreshToken);
            await _tokenRepo.SaveAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token.ToString()
            };
        }
    }
}