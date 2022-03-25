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
            return await httpClient.GetFromJsonAsync<List<User>>("api/User/all");
        }

        /* Add user */
        public async Task<HttpResponseMessage> RegisterUser(UserRegistrationDTO user)
        {
            
            var response = await httpClient.PostAsJsonAsync("api/User", user);
            return await response.Content.ReadFromJsonAsync<HttpResponseMessage>();
        
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
