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
            var response = httpClient.PostAsJsonAsync<UserRegistrationDTO>("api/Auth/Register", user);
            return await response;
        }

        public Task<HttpResponseMessage> Login(UserLoginDTO user)
        {
            throw new NotImplementedException();
        }
    }
}
