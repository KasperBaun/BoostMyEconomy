using BmeModels;
using MudBlazor;
using Newtonsoft.Json;

namespace BmeBlazorServer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        private List<Transaction> _AllUserTransactions { get; set; }
        public DateRange DateRange { get; set; } = new DateRange(new DateTime(DateTime.Now.Year, 1, 1), DateTime.Now.Date);
        public List<Transaction> UserTransactionsForPeriod { get; set; } = new List<Transaction>();
        public List<Transaction> IncomeTransactionsForPeriod { get; set; } = new List<Transaction>();
        public List <Transaction> ExpenseTransactionsForPeriod { get; set; } = new List<Transaction>();
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
            while(_AllUserTransactions == null)
            {
                _AllUserTransactions = await FetchUserTransactionsFromAPI();
            }
            UserTransactionsForPeriod = FilterTransactionsFromDateRange(DateRange);
            CalculateBalanceForPeriod();
            return true;
        }
        private List<Transaction> FilterTransactionsFromDateRange(DateRange range)
        {
            /*
            foreach(var transaction in _UserTransactions)
            {
                Console.WriteLine("$TransactionService.cs tDate: {0}, dStart: {1}, dEnd: {2}\n", 
                    DateOnly.Parse(transaction.MadeAt),
                    DateOnly.FromDateTime(range.Start.Value),
                    DateOnly.FromDateTime(range.End.Value)
                    );
            }
            */
            List<Transaction> list = new List<Transaction>();
            list = _AllUserTransactions.Where(x =>
            DateOnly.Parse(s: x.MadeAt) >= DateOnly.FromDateTime(range.Start.Value) 
            &&
            DateOnly.Parse(s: x.MadeAt) <= DateOnly.FromDateTime(range.End.Value)
            ).ToList();
            OnChange?.Invoke();
            return list;
        }
        public void PeriodChanged()
        {
            UserTransactionsForPeriod = FilterTransactionsFromDateRange(DateRange);
            CalculateBalanceForPeriod();
            OnChange?.Invoke();
        }
        private void CalculateBalanceForPeriod()
        {
            List<Transaction> incomeForPeriod =
                UserTransactionsForPeriod.Where(x => x.Type == "Income").ToList();
            int income = incomeForPeriod.Sum(x => x.Value);

            List<Transaction> expensesForPeriod =
                UserTransactionsForPeriod.Where(x => x.Type == "Expense").ToList();
            int expenses = expensesForPeriod.Sum(x => x.Value);
          
            //int result = (((income+expenses)*100/income));
            //Console.WriteLine("$TransactionService.cs - Income: {0}, Expenses: {1}, Income+Expenses: {2}, Balance: {3}", income, expenses, (income+expenses), result);   
            if (income == 0)
            {
                Balance = 0;
            }
            else
            {
                Balance = (income + expenses) * 100 / income;
            }
        }
        private void IncomeForPeriod()
        {
            IncomeTransactionsForPeriod = UserTransactionsForPeriod.Where(x =>
            x.Type=="Income"
            ).ToList();
        }
        private void ExpensesCurrentYear()
        {
            ExpenseTransactionsForPeriod = UserTransactionsForPeriod.Where(x =>
            x.Type == "Expense"
            ).ToList();
        }
    }
}
