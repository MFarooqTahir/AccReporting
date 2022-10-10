using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AccReporting.Server.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(s: _configuration[key: "securityKey"]);
            var secret = new SymmetricSecurityKey(key: key);

            return new SigningCredentials(key: secret, algorithm: SecurityAlgorithms.HmacSha256);
        }

        public async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new(type: ClaimTypes.Name, value: user.Email),
                new(type: ClaimTypes.NameIdentifier, value: user.Id)
        };

            var roles = await _userManager.GetRolesAsync(user: user);
            foreach (var role in roles)
            {
                claims.Add(item: new Claim(type: ClaimTypes.Role, value: role));
            }

            return claims;
        }

        public JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration[key: "validIssuer"],
                audience: _configuration[key: "validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(value: Convert.ToDouble(value: _configuration[key: "expiryInMinutes"])),
                signingCredentials: signingCredentials);

            return tokenOptions;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data: randomNumber);
                return Convert.ToBase64String(inArray: randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    key: Encoding.UTF8.GetBytes(s: _configuration[key: "securityKey"])),
                ValidateLifetime = false,
                ValidIssuer = _configuration[key: "validIssuer"],
                ValidAudience = _configuration[key: "validAudience"],
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token: token, validationParameters: tokenValidationParameters, validatedToken: out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(value: SecurityAlgorithms.HmacSha256,
                comparisonType: StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException(message: "Invalid token");
            }

            return principal;
        }
    }
}