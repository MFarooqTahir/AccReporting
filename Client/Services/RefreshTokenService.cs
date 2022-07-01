using Microsoft.AspNetCore.Components.Authorization;

namespace AccReporting.Client.Services
{
    public class RefreshTokenService
    {
        private readonly AuthenticationStateProvider _authProvider;
        private readonly IAuthenticationService _authService;

        public RefreshTokenService(AuthenticationStateProvider authProvider, IAuthenticationService authService)
        {
            _authProvider = authProvider;
            _authService = authService;
        }

        public async Task<string> TryRefreshToken()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user is not null)
            {
                var claims = user.Claims.ToList();
                var exp1 = user.FindFirst(c => c.Type.Equals("exp"));
                if (exp1 is not null)
                {
                    var exp = exp1.Value;
                    var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));

                    var timeUTC = DateTime.UtcNow;

                    var diff = expTime - timeUTC;
                    if (diff.TotalMinutes <= 2)
                        return await _authService.RefreshToken();

                }
            }
            return string.Empty;

        }
    }
}
