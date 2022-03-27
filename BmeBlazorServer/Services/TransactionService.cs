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
        public int Balance { get; set; } = 1;

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
            UserTransactions = _UserTransactions;
            FilterTransactionsFromDateRange(DateRange);
            CalculateBalanceForPeriod();
            return true;
        }
        private void FilterTransactionsFromDateRange(DateRange range)
        {
            UserTransactions = _UserTransactions.Where(x =>
            DateOnly.Parse(s: x.MadeAt) >= DateOnly.FromDateTime(range.Start.Value) 
            &&
            DateOnly.Parse(s: x.MadeAt) <= DateOnly.FromDateTime(range.End.Value)
            ).ToList();
            OnChange?.Invoke();
        }

        public void PeriodChanged()
        {
            FilterTransactionsFromDateRange(DateRange);
            CalculateBalanceForPeriod();
            OnChange?.Invoke();
        }

        private void CalculateBalanceForPeriod()
        {
            List<Transaction> incomeForPeriod =
                UserTransactions.Where(x => x.Type == "Income").ToList();
            int income = incomeForPeriod.Sum(x => x.Value);

            List<Transaction> expensesForPeriod =
                UserTransactions.Where(x => x.Type == "Expense").ToList();
            int expenses = expensesForPeriod.Sum(x => x.Value);
            int result = (((income+expenses)*100/income));
            //Console.WriteLine("$TransactionService.cs - Income: {0}, Expenses: {1}, Income+Expenses: {2}, Balance: {3}", income, expenses, (income+expenses), result);   
            if(expenses == 0)
            {
                Balance = 0;
            }
            else
            {
                Balance = (income + expenses) * 100 / income;
            }
        }
    }
}
