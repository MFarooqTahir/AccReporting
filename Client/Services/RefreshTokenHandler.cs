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
            if (!absPath.Contains(value: "token", comparisonType: StringComparison.InvariantCultureIgnoreCase) && !absPath.Contains(value: "auth", comparisonType: StringComparison.InvariantCultureIgnoreCase))
            {
                await _refreshTokenService.TryRefreshToken();

                var token = await _localStorage.GetItemAsync<string>(key: "authToken");

                if (!string.IsNullOrEmpty(value: token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(scheme: "bearer", parameter: token);
                }
            }

            return await base.SendAsync(request: request, cancellationToken: cancellationToken);
        }
    }
}