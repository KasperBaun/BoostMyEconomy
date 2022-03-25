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
            var response = httpClient.PostAsJsonAsync<UserRegistrationDTO>("api/Auth/Register", user);
            return await response;
        }

        public async Task<HttpResponseMessage> Login(UserLoginDTO user)
        {
            var response = httpClient.PostAsJsonAsync<UserLoginDTO>("api/Auth/Login", user);
            return await response;
        }

        public async Task<HttpResponseMessage> UserExists(string email)
        {   
            var response = httpClient.PostAsJsonAsync<string>("api/Auth/UserExists", email);
            return await response;
        }
    }
}
