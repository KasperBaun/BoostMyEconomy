using BmeModels;
using MudBlazor;
using Newtonsoft.Json;

namespace BmeBlazorServer.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        private List<Transaction> AllUserTransactions { get; set; } = new();
        private List<Category> Categories { get; set; } = new();
        public DateRange? PeriodSelected { get; set; } = new DateRange(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.Date);
        public List<Transaction> IncomeForPeriod { get; set; } = new List<Transaction>();
        public event Action? OnChange;
        public IncomeService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
        }
        private async Task FetchUserTransactionsFromAPI()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri: "api/Transaction/All");
            var token = await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                AllUserTransactions = await Task.FromResult(JsonConvert.DeserializeObject<List<Transaction>>(responseBody));
            }
        }
        private async Task FetchCategoriesFromAPI()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri: "api/Categories/All");
            var token = await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Categories = await Task.FromResult(JsonConvert.DeserializeObject<List<Category>>(responseBody));
            }
        }
        private List<Transaction> FilterTransactionsFromSelectedPeriod(DateRange periodSelected)
        {
            List<Transaction> list = new();
            list = AllUserTransactions.Where(x => 
            DateOnly.Parse(s: x.MadeAt) >= DateOnly.FromDateTime(periodSelected.Start.Value) &&
            DateOnly.Parse(s: x.MadeAt) <= DateOnly.FromDateTime(periodSelected.End.Value) &&
            x.Type=="Income"
            ).ToList();
            OnChange?.Invoke();
            return list;
        }
        public Task<bool> AddIncomeTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }
        public async Task<List<Transaction>> GetAllIncomeTransactions()
        {
            IncomeForPeriod.Clear();
            if(!AllUserTransactions.Any())
            {
                await FetchUserTransactionsFromAPI();
                IncomeForPeriod = FilterTransactionsFromSelectedPeriod(PeriodSelected);
                return IncomeForPeriod;
            }

            IncomeForPeriod = FilterTransactionsFromSelectedPeriod(PeriodSelected);
            return IncomeForPeriod;
        }
        public async Task<bool> InitializeService()
        {
            await FetchCategoriesFromAPI();
            await FetchUserTransactionsFromAPI();
            await GetAllIncomeTransactions();
            return true;
        }
        public async void PeriodChanged()
        {
            await InitializeService();
            OnChange?.Invoke();
        }
        public Task<bool> RemoveIncomeTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
