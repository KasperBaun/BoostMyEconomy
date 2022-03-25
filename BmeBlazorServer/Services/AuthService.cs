using BmeModels;

namespace BmeBlazorServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient httpClient;

        public AuthService(HttpClient _httpClient)
        {
            httpClient = _httpClient;
        }
     

        /* Add user */
        public async Task<HttpResponseMessage> RegisterUser(UserRegistrationDTO user)
        {
            // TODO : Setup these methods correctly so we dont have a possible null-reference
            var response = await httpClient.PostAsJsonAsync("api/Auth/Register", user);
            #pragma warning disable CS8603 // Possible null reference return.
            return await response.Content.ReadFromJsonAsync<HttpResponseMessage>();
            #pragma warning restore CS8603 // Possible null reference return.
        }

        public Task<HttpResponseMessage> Login(UserLoginDTO user)
        {
            throw new NotImplementedException();
        }
    }
}
