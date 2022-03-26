using BmeModels;
using Newtonsoft.Json;

namespace BmeBlazorServer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient httpClient;

        private ILocalStorageService localStorageService;

        public TransactionService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
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

        public async Task<List<Transaction>> GetAllUserTransactions()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri: "api/Transaction/All");
            var token = await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return await Task.FromResult(JsonConvert.DeserializeObject<List<Transaction>>(responseBody));
            }
            else
                return null;
        }
    }
}
