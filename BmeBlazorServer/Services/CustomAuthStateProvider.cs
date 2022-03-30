using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace BmeBlazorServer.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        public CustomAuthStateProvider (ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await _localStorage.GetItemAsStringAsync("token");
            
            var identity = new ClaimsIdentity();
            

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var tokenContent = handler.ReadJwtToken(token);
                if (tokenContent.Claims.Any())
                {
                    int timestamp = tokenContent.Payload.Exp.Value;
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
        public static DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp); //
        }
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
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
    }
}
