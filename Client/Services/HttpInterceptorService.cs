using System.Net.Http.Headers;

using Toolbelt.Blazor;

namespace AccReporting.Client.Services
{
    public class HttpInterceptorService
    {
        private readonly HttpClientInterceptor _interceptor;
        private readonly RefreshTokenService _refreshTokenService;

        public HttpInterceptorService(HttpClientInterceptor interceptor, RefreshTokenService refreshTokenService)
        {
            _interceptor = interceptor;
            _refreshTokenService = refreshTokenService;
        }

        public void RegisterEvent()
        {
            _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync; _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;
        }

        public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            var absPath = e.Request?.RequestUri?.AbsolutePath;
            if (!string.IsNullOrWhiteSpace(value: absPath) && !absPath.Contains(value: "token") && !absPath.Contains(value: "auth"))
            {
                var token = await _refreshTokenService.TryRefreshToken();

                if (!string.IsNullOrEmpty(value: token))
                {
                    e.Request.Headers.Authorization = new AuthenticationHeaderValue(scheme: "bearer", parameter: token);
                }
            }
        }
    }
}