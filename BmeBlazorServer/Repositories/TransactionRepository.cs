using BmeModels;
using Newtonsoft.Json;
using BmeBlazorServer.Services;

namespace BmeBlazorServer.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        public event Action? OnChange;
        private List<Transaction> UserTransactions { get; set; } = new();
        private List<Category> Categories { get; set; } = new();
        private readonly HttpClient httpClient;
        private readonly ICategoryRepository categoryRepository;

        public TransactionRepository(HttpClient _httpClient, ICategoryRepository _categoryRepository)
        {
            httpClient = _httpClient;
            categoryRepository = _categoryRepository;
        }

        public async Task<ResponseModel> CreateTransaction(TransactionDTO transaction)
        {
            ResponseModel responseModel = new();
            HttpRequestMessage? requestMessage = new(HttpMethod.Post, requestUri: "api/Transaction/");
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(transaction),System.Text.Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization = AuthStateProvider.TokenBearer;

            var response = await httpClient.SendAsync(requestMessage);
            if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseContent = await Task.FromResult(JsonConvert.DeserializeObject<Transaction>(responseBody));
                responseModel.Status = true;
                Console.WriteLine(response.ReasonPhrase + response.Content+"\n");
                responseModel.Message = response.ReasonPhrase + response.Content;
                UserTransactions.Add(responseContent);
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

        public async Task<ResponseModel> DeleteTransaction(Transaction transaction)
        {
            ResponseModel responseModel = new();
            HttpRequestMessage? requestMessage = new(HttpMethod.Delete, requestUri: "api/Transaction/");
            requestMessage.Content = new StringContent(
                JsonConvert.SerializeObject(transaction), System.Text.Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization = AuthStateProvider.TokenBearer;

            var response = await httpClient.SendAsync(requestMessage);
            if ((response.StatusCode == System.Net.HttpStatusCode.OK) || (response.StatusCode == System.Net.HttpStatusCode.NoContent))
            {
                responseModel.Status = true;
                responseModel.Message = response.ReasonPhrase + response.Content;
                UserTransactions.Remove(transaction);
                OnChange?.Invoke();
                return responseModel;
            }
            
            Console.WriteLine(response.ReasonPhrase + response.Content + "\n");
            responseModel.Status = false;
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseContent = await Task.FromResult(JsonConvert.DeserializeObject<List<Transaction>>(responseBody));
            responseModel.Message = response.ReasonPhrase + responseContent;
            return responseModel;
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            if (!UserTransactions.Any())
            {
                await FetchUserTransactions();
            }
            return UserTransactions;
        }

        public async Task<ResponseModel> UpdateTransaction(Transaction transaction)
        {
            ResponseModel responseModel = new();
            HttpRequestMessage? requestMessage = new(HttpMethod.Put, requestUri: "api/Transaction/");
            requestMessage.Content = new StringContent(
                JsonConvert.SerializeObject(transaction), System.Text.Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization = AuthStateProvider.TokenBearer;

            var response = await httpClient.SendAsync(requestMessage);
            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                responseModel.Status = true;
                //Console.WriteLine(response.ReasonPhrase + response.Content.ReadAsStream(). + "\n");
                responseModel.Message = response.ReasonPhrase + response.Content;
                var index = UserTransactions.FindIndex(x => x.Id == transaction.Id);
                if(index > -1)
                {
                    UserTransactions[index] = transaction;
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
                responseModel.Message = response.StatusCode.ToString()+" " + response.RequestMessage;
                return responseModel;
            }
        }

        private async Task FetchUserTransactions()
        {
            Categories = await categoryRepository.GetCategories();
        
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri: "api/Transaction/All");
            requestMessage.Headers.Authorization = AuthStateProvider.TokenBearer;
            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseContent = await Task.FromResult(JsonConvert.DeserializeObject<List<Transaction>>(responseBody));
                if (responseContent != null)
                {   /*
                    foreach(var transaction in responseContent)
                    {
                        Console.WriteLine(transaction.ToString());
                    }
                    */
                    UserTransactions = responseContent;
                }
                else
                {
                    Console.WriteLine("$TransactionService.cs@FetchUserTransactions(): failed fetching transactions from WebAPI");
                    UserTransactions.Clear();
                }
            }
        }
    }
}
