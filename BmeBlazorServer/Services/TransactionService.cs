using BmeModels;
using MudBlazor;
using Newtonsoft.Json;

namespace BmeBlazorServer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        private List<Transaction> _UserTransactions { get; set; }
        public DateRange DateRange { get; set; } = new DateRange(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.Date);
        public List<Transaction> UserTransactions { get; set; } = new List<Transaction>();
        public event Action OnChange;

        public TransactionService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
        }

 
        // StartMonth="@DateTime.Now.AddMonths(-1)"


        public async Task<List<Transaction>> FetchUserTransactionsFromAPI()
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

        public async Task<bool> GetAllUserTransactions()
        {
            while(_UserTransactions == null)
            {
                _UserTransactions = await FetchUserTransactionsFromAPI();
            }
            FilterTransactionsFromDateRange(DateRange);
            UserTransactions = _UserTransactions;
            return true;
        }
        private void FilterTransactionsFromDateRange(DateRange range)
        {
            UserTransactions =  _UserTransactions.FindAll(e =>
            DateOnly.FromDateTime(DateTime.Parse(e.MadeAt)) >= DateOnly.FromDateTime(range.Start.Value)
            &&
            DateOnly.FromDateTime(DateTime.Parse(e.MadeAt)) <= DateOnly.FromDateTime(range.End.Value)
            );
        }

        public void SetDateRange(DateRange dateRange)
        {
            DateRange = dateRange;
            FilterTransactionsFromDateRange(DateRange);
            OnChange?.Invoke();
        }
    }
}
