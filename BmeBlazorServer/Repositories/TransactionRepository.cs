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
        private readonly ILocalStorageService localStorageService;
        private readonly ICategoryRepository categoryRepository;

        public TransactionRepository(HttpClient _httpClient, ILocalStorageService _localStorageService, ICategoryRepository _categoryRepository)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
            categoryRepository = _categoryRepository;
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
            var token = await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(requestMessage);
            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                responseModel.Status = true;
                Console.WriteLine(response.ReasonPhrase + response.Content + "\n");
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
                responseModel.Message = "Something went wrong";
                return responseModel;
            }
        }

        private async Task FetchUserTransactions()
        {
            Categories = await categoryRepository.GetCategories();
            //foreach(Category c in Categories)
            //{
            //    Console.WriteLine(c.Title);
            //}
            /*
            for(int i = 0; i<50; i++)
            {
                TransactionDTO transactionDTO = new()
                {
                    CategoryId = 1,
                    Description = "Hej",
                    MadeAt = DateTime.Now,
                    Source = "Novozymes",
                    SubcategoryId = 0,
                    Type ="Income",
                    UserId = 3,
                    Value = 4500+i
                };
                CreateTransaction(transactionDTO);
            }
            */
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri: "api/Transaction/All");
            requestMessage.Headers.Authorization = AuthStateProvider.TokenBearer;
            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseContent = await Task.FromResult(JsonConvert.DeserializeObject<List<Transaction>>(responseBody));
                if (responseContent != null)
                {
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
