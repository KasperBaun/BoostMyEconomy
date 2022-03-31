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
        public DateRange? PeriodSelected { get; set; }
        public List<Transaction> IncomeForPeriod { get; set; } = new List<Transaction>();
        public event Action? OnChange;
        public IncomeService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
            PeriodSelected = new DateRange(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.Date);
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
                var userTransactions = JsonConvert.DeserializeObject<List<Transaction>>(responseBody);
                if(userTransactions != null)
                {
                    AllUserTransactions = userTransactions;
                }
                else
                {
                    AllUserTransactions.Clear();
                    throw new Exception("$IncomeService.cs@FetchUserTransactions(): Fetch failed");
                }
            }
        }
        private async Task FetchCategories()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri: "api/Categories/All");
            var token = await localStorageService.GetItemAsync<string>("token");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<Category>>(responseBody);
                if(categories != null)
                {
                    Categories = categories;
                }
                else
                {
                    Categories = new List<Category>();
                    throw new Exception("$IncomeService.cs@FetchCategories(): Fetch failed");
                }
            }
        }
        private List<Transaction> FilterTransactionsFromSelectedPeriod(DateRange periodSelected)
        {
            List<Transaction> list = new();
            if(periodSelected.Start.HasValue && periodSelected.End.HasValue)
            {
                DateOnly Start = DateOnly.FromDateTime(periodSelected.Start.Value);
                DateOnly End = DateOnly.FromDateTime(periodSelected.End.Value);
                list = AllUserTransactions.Where(x => 
                DateOnly.Parse(s: x.MadeAt) >= Start && DateOnly.Parse(s: x.MadeAt) <= End && x.Type=="Income").ToList();
                return list;
            }
            else
            {
                return list;
            }
        }
        public async Task<bool> InitializeService()
        {
            await FetchCategories();
            await FetchUserTransactions();

            IncomeForPeriod = FilterTransactionsFromSelectedPeriod(PeriodSelected);
            OnChange?.Invoke();
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
        public Task<bool> AddIncomeTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
