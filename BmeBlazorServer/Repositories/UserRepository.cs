using BmeModels;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BmeBlazorServer.Services;

namespace BmeBlazorServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        public User CurrentUser { get; set; } = new();
        public event Action? OnChange;

        public UserRepository(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
        }
        /*
        private async void ParseLoggedInUserName()
        {
            var token = await localStorageService.GetItemAsync<string>("token");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            var nameClaim = tokenS.Claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault();
            var username = nameClaim.Value;
            CurrentUser.FirstName = username.Split(' ')[0];
            CurrentUser.LastName = username.Split(' ')[1];
        }*/
        private async Task<int> ParseLoggedInUserId()
        {
            var token = await localStorageService.GetItemAsync<string>("token");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            if (jsonToken is JwtSecurityToken tokenS)
            {
                var nameClaim = tokenS.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                if (nameClaim != null)
                {
                    int userId = int.Parse(nameClaim.Value);
                    return userId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        public async Task<List<User>> GetUsers()
        {
            List<User> users = new();    
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri: "api/User/All");
            requestMessage.Headers.Authorization = AuthStateProvider.TokenBearer;
            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiUsers = JsonConvert.DeserializeObject<List<User>>(responseBody);
                if(apiUsers != null)
                {
                    return apiUsers;
                }
                else
                {
                    Console.WriteLine("$UserService.cs@GetUsers(): failed fetching users from api");
                    return users;
                }
            }
            else
            {
                Console.WriteLine("$UserService.cs@GetUsers(): failed fetching users from api");
                return users;
            }
        }
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
        {  
            return await httpClient.PutAsJsonAsync("api/User/", user);
        }
        public async Task<User> GetCurrentUser()
        {
            if(CurrentUser == null)
            {
                await FetchCurrentUser();
            }

            return CurrentUser;
        }
        private async Task<bool> FetchCurrentUser()
        {
            try
            {
                {
                    int userId = await ParseLoggedInUserId();
                    string requestUri = "api/User/"+userId;
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                    requestMessage.Headers.Authorization = AuthStateProvider.TokenBearer;
                    var response = await httpClient.SendAsync(requestMessage);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var responseUser = await Task.FromResult(JsonConvert.DeserializeObject<User>(responseBody));
                        if (responseUser != null)
                        {
                            CurrentUser = responseUser;
                            //Console.WriteLine(responseUser.ToString());
                            OnChange?.Invoke();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return false;
        }
    }
}
