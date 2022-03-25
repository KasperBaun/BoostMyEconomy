using BmeModels;

namespace BmeBlazorServer.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient httpClient;

        public UserService(HttpClient _httpClient)
        {
            httpClient = _httpClient;
        }
        /* Get all users */
        public async Task<List<User>> GetUsers()
        {
            // TODO : Setup these methods correctly so we dont have a possible null-reference
#pragma warning disable CS8603 // Possible null reference return.
            return await httpClient.GetFromJsonAsync<List<User>>(requestUri: "api/User/All");
#pragma warning restore CS8603 // Possible null reference return.
        }

        /* Add user */
        public async Task<HttpResponseMessage> RegisterUser(UserRegistrationDTO user)
        {
            // TODO : Setup these methods correctly so we dont have a possible null-reference
            var response = await httpClient.PostAsJsonAsync("api/User/Register", user);
#pragma warning disable CS8603 // Possible null reference return.
            return await response.Content.ReadFromJsonAsync<HttpResponseMessage>();
#pragma warning restore CS8603 // Possible null reference return.

        }

        /* Delete user */
        public async Task<HttpResponseMessage> DeleteUser(int userId)
        {
            return await httpClient.DeleteAsync("api/User/" + userId);
        }

        public async Task<HttpResponseMessage> UpdateUser(User user)
        {   /*
            var response = await httpClient.PutAsJsonAsync("api/Users", user);
            return await response.Content.ReadFromJsonAsync<HttpResponseMessage>();
            */
            return await httpClient.PutAsJsonAsync("api/User/", user);
        }
    }
}
