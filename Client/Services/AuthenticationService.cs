using AccReporting.Client.Classes;
using AccReporting.Shared.DTOs;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AccReporting.Client.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthenticationService(HttpClient client, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
        {
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
        }

        public async Task<string> RefreshToken()
        {
            var token = await _localStorage.GetItemAsync<string>(key: "authToken");
            var refreshToken = await _localStorage.GetItemAsync<string>(key: "refreshToken");

            var refreshResult = await _client.PostAsJsonAsync(requestUri: "/api/Token/refresh", value: new RefreshTokenDto { Token = token, RefreshToken = refreshToken });
            var refreshContent = await refreshResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponseDto>(json: refreshContent, options: _options);

            if (!refreshResult.IsSuccessStatusCode)
                throw new ApplicationException(message: "Something went wrong during the refresh token action");

            await _localStorage.SetItemAsync(key: "authToken", data: result.Token);
            await _localStorage.SetItemAsync(key: nameof(refreshToken), data: result.RefreshToken);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "bearer", parameter: result.Token);

            return result.Token;
        }

        public async Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration)
        {
            var registrationResult = await _client.PostAsJsonAsync(requestUri: "api/Auth/Registration", value: userForRegistration);
            var registrationContent = await registrationResult.Content.ReadAsStringAsync();
            if (!registrationResult.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<RegistrationResponseDto>(json: registrationContent, options: _options);
                return result;
            }
            return new RegistrationResponseDto { IsSuccessfulRegistration = true };
        }

        public async Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication)
        {
            var authResult = await _client.PostAsJsonAsync(requestUri: "api/Auth/Login", value: userForAuthentication);
            var authContent = await authResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponseDto>(json: authContent, options: _options);

            if (!authResult.IsSuccessStatusCode)
                return result;

            await _localStorage.SetItemAsync(key: "authToken", data: result.Token);
            await _localStorage.SetItemAsync(key: "refreshToken", data: result.RefreshToken);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "bearer", parameter: result.Token);
            ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication();

            return new AuthResponseDto { IsAuthSuccessful = true };
        }

        public async Task Logout()
        {
            await _localStorage.ClearAsync();
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            _client.DefaultRequestHeaders.Authorization = null;
        }
    }
}