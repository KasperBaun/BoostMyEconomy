using BmeModels;
using MudBlazor;
using Newtonsoft.Json;

namespace BmeBlazorServer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        private List<Transaction> _UserTransactions;
        private DateRange _dateRange { get; set; }
        public List<Transaction> UserTransactions { get => _UserTransactions; set => _UserTransactions = value; }
        public event Action OnChange;

        public TransactionService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
            //UserTransactions = FetchUserTransactionsFromAPI().Result;
            //_dateRange = new DateRange(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.Date);
            //FilterTransactionsFromDateRange(_dateRange, UserTransactions);
            //OnChange?.Invoke();
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

        public async Task<List<Transaction>> GetAllUserTransactions()
        {
            return null;
        }
        private void FilterTransactionsFromDateRange(DateRange range, List<Transaction> list)
        {
            list = UserTransactions.FindAll(e =>
            DateOnly.FromDateTime(DateTime.Parse(e.MadeAt)) >= DateOnly.FromDateTime(range.Start.Value)
            &&
            DateOnly.FromDateTime(DateTime.Parse(e.MadeAt)) <= DateOnly.FromDateTime(range.End.Value));
        }

        public DateRange GetDateRange()
        {
            return _dateRange;
        }

        public void SetDateRange(DateRange dateRange)
        {
            _dateRange = dateRange;
        }
    }
}
