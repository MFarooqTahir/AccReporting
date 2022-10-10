using AccReporting.Server.Services;
using AccReporting.Shared.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.IdentityModel.Tokens.Jwt;

namespace AccReporting.Server.Controllers
{
    [Route(template: "api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost(template: "Registration")]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();
                var user = new ApplicationUser { UserName = userForRegistration.Email, Email = userForRegistration.Email };

                var result = await _userManager.CreateAsync(user: user, password: userForRegistration.Password);
                if (result.Succeeded) return StatusCode(statusCode: 201);
                var errors = result.Errors.Select(selector: e => e.Description);
                return BadRequest(error: new RegistrationResponseDto { Errors = errors });
            }
            catch (Exception ex)
            {
                return BadRequest(error: new RegistrationResponseDto { Errors = new[] { ex.InnerException!.Message } });
            }
        }

        [HttpPost(template: nameof(Login))]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            var user = await _userManager.FindByNameAsync(userName: userForAuthentication.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user: user, password: userForAuthentication.Password))
                return Unauthorized(value: new AuthResponseDto { ErrorMessage = "Invalid Authentication" });

            var signingCredentials = _tokenService.GetSigningCredentials();
            var claims = await _tokenService.GetClaims(user: user);
            var tokenOptions = _tokenService.GenerateTokenOptions(signingCredentials: signingCredentials, claims: claims);
            var token = new JwtSecurityTokenHandler().WriteToken(token: tokenOptions);
            user.RefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(value: 7);
            await _userManager.UpdateAsync(user: user);
            return Ok(value: new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }
    }
}