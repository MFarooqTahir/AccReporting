using System.Security.Claims;
using System.Text.Json;

namespace AccReporting.Client.Classes
{
    public static class JwtParser
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split(separator: '.')[1];
            var jsonBytes = ParseBase64WithoutPadding(base64: payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(utf8Json: jsonBytes);

            keyValuePairs.TryGetValue(key: ClaimTypes.Role, value: out var roles);

            if (roles is not null)
            {
                if (roles.ToString().Trim().StartsWith(value: "["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(json: roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(item: new Claim(type: ClaimTypes.Role, value: parsedRole));
                    }
                }
                else
                {
                    claims.Add(item: new Claim(type: ClaimTypes.Role, value: roles.ToString()));
                }

                keyValuePairs.Remove(key: ClaimTypes.Role);
            }

            claims.AddRange(collection: keyValuePairs.Select(selector: kvp => new Claim(type: kvp.Key, value: kvp.Value.ToString())));

            return claims;
        }

        public static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(s: base64);
        }
    }
}