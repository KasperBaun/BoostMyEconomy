using BmeModels;
using MudBlazor;
using Newtonsoft.Json;

namespace BmeBlazorServer.Services
{
    public class OverviewService : IOverviewService
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        private List<Transaction> AllUserTransactions { get; set; }
        private List<Category> AllCategories { get; set; }
        public DateTime? YearSelected { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public List<Transaction> UserTransactionsForPeriod { get; set; } = new List<Transaction>();
        public List<ChartSeries> IncomeForYear { get; set; } = new();
        public string SumIncomeForYear { get; set; }
        public List <Transaction> ExpenseTransactionsForPeriod { get; set; } = new List<Transaction>();
        public int Balance { get; set; } = 1;

        public event Action OnChange;

        public OverviewService(HttpClient _httpClient, ILocalStorageService _localStorageService)
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
                AllCategories = await Task.FromResult(JsonConvert.DeserializeObject<List<Category>>(responseBody));
            }
        }


        public async Task<bool> InitializeOverviewService()
        {
            while(AllCategories == null)
            {
                await FetchCategoriesFromAPI();
            }
            while(AllUserTransactions == null)
            {
                await FetchUserTransactionsFromAPI();
            }
            UserTransactionsForPeriod = FilterTransactionsFromSelectedYear(YearSelected.Value);
            CalculateBalanceForPeriod();
            await IncomeForPeriod();
            SumIncomeForYear = CalculateSumForYear(IncomeForYear);
            OnChange?.Invoke();
            return true;
        }
        private List<Transaction> FilterTransactionsFromSelectedYear(DateTime yearSelected)
        {
            List<Transaction> list = new();
            list = AllUserTransactions.Where(x => DateOnly.Parse(s: x.MadeAt).Year == yearSelected.Year).ToList();
            OnChange?.Invoke();
            return list;
        }
        public async void PeriodChanged()
        {
            UserTransactionsForPeriod = FilterTransactionsFromSelectedYear(YearSelected.Value);
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
        private async Task IncomeForPeriod()
        {
            List<Transaction> list = UserTransactionsForPeriod.Where(x => x.Type=="Income").ToList();
            ChartSeries series = new ChartSeries();
            double[] data = new double[12];
            for(int i=0; i < 12; i++)
            {
                List<Transaction> monthlyList = list.Where(x => DateTime.Parse(x.MadeAt).Month == i).ToList();
                double monthlySum = monthlyList.Sum(x => x.Value);
                Console.WriteLine("$OverviewService.cs_IncomeForPeriod(): monthlysum for {0} is {1}",i,monthlySum);
                data[i] = monthlySum/1000;
            }
            IncomeForYear.Add(new ChartSeries() { Name = "Income", Data = data });
        }
        private void ExpensesCurrentYear()
        {
            ExpenseTransactionsForPeriod = UserTransactionsForPeriod.Where(x =>
            x.Type == "Expense"
            ).ToList();
        }

        private string CalculateSumForYear(List<ChartSeries> list)
        {
            double sum = 0;
            foreach(ChartSeries series in list)
            {
                sum = series.Data.ToList().Sum();
            }
            //Console.WriteLine(sum);
            int thousands = (int)sum;
            int hundreds = (int)sum %10;
            string result = thousands+"."+hundreds;

            return result;
        }
    }
}





