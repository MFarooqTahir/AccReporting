using Blazored.LocalStorage;

using System.Net.Http.Headers;

namespace AccReporting.Client.Services
{
    public class RefreshTokenHandler : DelegatingHandler
    {
        private readonly RefreshTokenService _refreshTokenService;
        private readonly ILocalStorageService _localStorage;

        public RefreshTokenHandler(RefreshTokenService refreshTokenService, ILocalStorageService localStorage)
        {
            _refreshTokenService = refreshTokenService;
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var absPath = request.RequestUri.AbsolutePath;
            if (!absPath.Contains("token", StringComparison.InvariantCultureIgnoreCase) && !absPath.Contains("auth", StringComparison.InvariantCultureIgnoreCase))
            {
                await _refreshTokenService.TryRefreshToken();

                var token = await _localStorage.GetItemAsync<string>("authToken");

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}