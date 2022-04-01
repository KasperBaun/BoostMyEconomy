using BmeModels;
using Newtonsoft.Json;

namespace BmeBlazorServer.Services
{
    public class TransactionService : ITransactionService
    {
        public event Action? OnChange;
        private List<Transaction> AllUserTransactions { get; set; } = new();
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;

        public TransactionService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
        }

        public async Task<ResponseModel> CreateTransaction(TransactionDTO transaction)
        {
            ResponseModel responseModel = new();
            HttpRequestMessage? requestMessage = new(HttpMethod.Post, requestUri: "api/Transaction/");
            requestMessage.Content = new StringContent(
                JsonConvert.SerializeObject(transaction),System.Text.Encoding.UTF8, "application/json");
            var token = await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(requestMessage);
            if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                responseModel.Status = true;
                Console.WriteLine(response.ReasonPhrase + response.Content+"\n");
                responseModel.Message = response.ReasonPhrase + response.Content;
                await FetchUserTransactions();
                OnChange?.Invoke();
                return responseModel;
            }
            if( response.StatusCode == System.Net.HttpStatusCode.Conflict || 
                response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine(response.ReasonPhrase + response.Content + "\n");
                responseModel.Status = false;
                responseModel.Message = response.ReasonPhrase + response.Content;
                return responseModel;
            }
            else
            {
                Console.WriteLine(response.ReasonPhrase + response.Content + "\n");
                responseModel.Status = false;
                responseModel.Message = "Something went wrong";
                return responseModel;
            }
        }

        public async Task<ResponseModel> DeleteTransaction(int transactionId)
        {
            ResponseModel responseModel = new();
            HttpRequestMessage? requestMessage = new(HttpMethod.Delete, requestUri: "api/Transaction/"+transactionId);
            var token = await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(requestMessage);
            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                responseModel.Status = true;
                //Console.WriteLine(response.ReasonPhrase + response.Content + "\n");
                responseModel.Message = response.ReasonPhrase + response.Content;
                //var index = AllUserTransactions.FindIndex(x => x.Id == transactionId);
                await FetchUserTransactions();
                //AllUserTransactions.RemoveAt(index);
                OnChange?.Invoke();
                return responseModel;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
                response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine(response.ReasonPhrase + response.Content + "\n");
                responseModel.Status = false;
                responseModel.Message = response.ReasonPhrase + response.Content;
                return responseModel;
            }
            else
            {
                Console.WriteLine(response.ReasonPhrase + response.Content + "\n");
                responseModel.Status = false;
                responseModel.Message = "Something went wrong";
                return responseModel;
            }
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            if (!AllUserTransactions.Any())
            {
                await FetchUserTransactions();
            }
            return AllUserTransactions;
        }

        public async Task<ResponseModel> UpdateTransaction(Transaction transaction)
        {
            ResponseModel responseModel = new();
            HttpRequestMessage? requestMessage = new(HttpMethod.Put, requestUri: "api/Transaction/");
            requestMessage.Content = new StringContent(
                JsonConvert.SerializeObject(transaction), System.Text.Encoding.UTF8, "application/json");
            var token = await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(requestMessage);
            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                responseModel.Status = true;
                Console.WriteLine(response.ReasonPhrase + response.Content + "\n");
                responseModel.Message = response.ReasonPhrase + response.Content;
                var index = AllUserTransactions.FindIndex(x => x.Id == transaction.Id);
                if(index > -1)
                {
                    AllUserTransactions[index] = transaction;
                }
                OnChange?.Invoke();
                return responseModel;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict ||
                response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine(response.ReasonPhrase + response.Content + "\n");
                responseModel.Status = false;
                responseModel.Message = response.ReasonPhrase + response.Content;
                return responseModel;
            }
            else
            {
                Console.WriteLine(response.ReasonPhrase + response.Content + "\n");
                responseModel.Status = false;
                responseModel.Message = "Something went wrong";
                return responseModel;
            }
        }

        private async Task FetchUserTransactions()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri: "api/Transaction/All");
            var token = await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseContent = await Task.FromResult(JsonConvert.DeserializeObject<List<Transaction>>(responseBody));
                if (responseContent != null)
                {
                    AllUserTransactions = responseContent;
                }
                else
                {
                    Console.WriteLine("$TransactionService.cs@FetchUserTransactions(): failed fetching transactions from WebAPI");
                    AllUserTransactions.Clear();
                }
            }
        }
    }
}
