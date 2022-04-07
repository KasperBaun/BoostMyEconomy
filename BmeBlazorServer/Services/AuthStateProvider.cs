using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace BmeBlazorServer.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public static AuthenticationHeaderValue? TokenBearer { get; set; }
        public AuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string jwt = await _localStorage.GetItemAsStringAsync("token");
            var identity = new ClaimsIdentity();


            if (!string.IsNullOrEmpty(jwt))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                if (token.Claims.Any())
                {
                    int timestamp;
                    if (token.Payload.Exp.HasValue)
                    {
                        timestamp = (token.Payload.Exp.Value);
                        DateTime date = ConvertFromUnixTimestamp(timestamp);
                        if (date > DateTimeOffset.UtcNow)
                        {
                            IEnumerable<Claim> claimsList = token.Claims.ToList();
                            identity = new ClaimsIdentity(claimsList, "jwt");
                            TokenBearer = new AuthenticationHeaderValue("Bearer", jwt);
                        }
                        else
                        {
                            await _localStorage.RemoveItemAsync("token");
                            identity = new ClaimsIdentity();
                        }
                    }
                    else
                    {
                        await _localStorage.RemoveItemAsync("token");
                        throw new Exception("$CustomAuthStateProvider.cs@GetAuthenticationStateAsync: tokenContent.Payload.Exp error");
                    }
                }
            }
            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);
            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }
        public static DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            DateTime origin = new(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }



        /*

                var handler = new JwtSecurityTokenHandler();
                var tokenContent = handler.ReadJwtToken(token);
                if (tokenContent.Claims.Any())
                {
                    int timestamp;
                    if (tokenContent.Payload.Exp.HasValue)
                    {
                        timestamp = (tokenContent.Payload.Exp.Value);
                    }
                    else
                    {
                        throw new Exception("$CustomAuthStateProvider.cs@GetAuthenticationStateAsync: tokenContent.Payload.Exp error");
                    }

                    DateTime date = ConvertFromUnixTimestamp(timestamp);
                    if(date > DateTimeOffset.UtcNow)
                    {
                        IEnumerable<Claim> claimsList = ParseClaimsFromJwt(token);
                        identity = new ClaimsIdentity(claimsList, "jwt");
                    }
                    else
                    {
                        await _localStorage.RemoveItemAsync("token");
                        identity = new ClaimsIdentity();
                    }

                }
                else
                {
                    await _localStorage.RemoveItemAsync("token");
                    identity = new ClaimsIdentity();
                }

            }

            var user = new ClaimsPrincipal(identity);

            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }
        
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            if(keyValuePairs != null)
            {
                List<Claim> claims = new();
                foreach (var keyValue in keyValuePairs)
                {
                    if(keyValue.Value != null)
                    {
                        string value = keyValue.Value.ToString();
                        claims.Add(new Claim(keyValue.Key, value));
                    }
                }
                return claims;
            }
            else
            {
                return Enumerable.Empty<Claim>();
            }

        }
        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
        */
    }
}
