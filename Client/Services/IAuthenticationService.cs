using AccReporting.Shared.DTOs;

namespace AccReporting.Client.Services
{
    public interface IAuthenticationService
    {
        Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration);

        Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication);

        Task<string> RefreshToken();

        Task Logout();
    }
}