using BmeModels;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BmeBlazorServer.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        private User currentUser { get; set; }
        public string UserName { get; set; }
        public event Action OnChange;

        public UserService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
        }

        public async void ParseLoggedInUserName()
        {
            var token = await localStorageService.GetItemAsync<string>("token");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            var nameClaim = tokenS.Claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault();
            var username = nameClaim.Value;
            //Console.WriteLine("$UserService.cs - Username: "+ username);
            UserName = username;
            OnChange?.Invoke();
        }

        /* Get all users */
        public async Task<List<User>> GetUsers()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri: "api/User/All");
            var token =  await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return await Task.FromResult(JsonConvert.DeserializeObject<List<User>>(responseBody));
            }
            else
                return null;
        }

        /* Delete user */
        public async Task<HttpResponseMessage> DeleteUser(int userId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri: "api/User/"+userId);
            var token = await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(requestMessage);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateUser(User user)
        {   /*
            var response = await httpClient.PutAsJsonAsync("api/Users", user);
            return await response.Content.ReadFromJsonAsync<HttpResponseMessage>();
            */
            return await httpClient.PutAsJsonAsync("api/User/", user);
        }

        public async Task<User> GetCurrentUser()
        {
            if(currentUser == null)
            {
                ParseLoggedInUserName();
            }
            return currentUser;
        }
    }
}
