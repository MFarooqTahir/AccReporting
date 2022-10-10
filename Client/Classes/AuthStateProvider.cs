using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using System.Net.Http.Headers;
using System.Security.Claims;

namespace AccReporting.Client.Classes
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(key: "authToken");
            if (string.IsNullOrWhiteSpace(value: token))
                return new AuthenticationState(user: new ClaimsPrincipal(identity: new ClaimsIdentity()));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "bearer", parameter: token);
            var x = JwtParser.ParseClaimsFromJwt(jwt: token);
            return new AuthenticationState(user: new ClaimsPrincipal(
                identity: new ClaimsIdentity(claims: x, authenticationType: "jwtAuthType")));
        }

        public void NotifyUserAuthentication()
        {
            var authState = GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(task: authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(result: new AuthenticationState(user: new ClaimsPrincipal(identity: new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(task: authState);
        }
    }
}