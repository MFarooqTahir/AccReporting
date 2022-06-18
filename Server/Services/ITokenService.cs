using AccReporting.Shared.ContextModels;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AccReporting.Server.Services
{
    public interface ITokenService
    {
        string GenerateRefreshToken();
        JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims);
        Task<List<Claim>> GetClaims(ApplicationUser user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        SigningCredentials GetSigningCredentials();
    }
}