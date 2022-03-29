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
        private List<Category> Categories { get; set; }
        public DateTime? YearSelected { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public List<Transaction> TransactionsForPeriod { get; set; } = new List<Transaction>();
        public List<double> IncomePrMonth { get; set; } = new();
        public List<double> ExpensesPrMonth { get; set; } = new();
        public List<double> ResultPrMonth { get; set; } = new();
        public List<double> ResultPrMonthAcc { get; set; } = new();
        public List <Result> Results { get; set; } = new();
        public List<ChartSeries> IncomeAndExpense { get; set; } = new();
        public List<string> GetMonths { get; set; } = new(){ "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public string SumIncome { get; set; }
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
                Categories = await Task.FromResult(JsonConvert.DeserializeObject<List<Category>>(responseBody));
            }
        }
        public async Task<bool> InitializeOverviewService()
        {
            while(Categories == null)
            {
                await FetchCategoriesFromAPI();
            }
            while(AllUserTransactions == null)
            {
                await FetchUserTransactionsFromAPI();
            }
            TransactionsForPeriod = FilterTransactionsFromSelectedYear(YearSelected.Value);
            CalculateBalanceForPeriod();
            IncomeForPeriod();
            SumIncome = CalculateSumForYear(IncomePrMonth);
            ExpensesForPeriod();
            ResultForPeriod();
            ResultForPeriodAcc();
            GenerateResultsList();
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
            //TransactionsForPeriod = FilterTransactionsFromSelectedYear(YearSelected.Value);
            //CalculateBalanceForPeriod();
            await InitializeOverviewService();
            OnChange?.Invoke();
        }
        private void CalculateBalanceForPeriod()
        {
            List<Transaction> incomeForPeriod =
                TransactionsForPeriod.Where(x => x.Type == "Income").ToList();
            int income = incomeForPeriod.Sum(x => x.Value);

            List<Transaction> expensesForPeriod =
                TransactionsForPeriod.Where(x => x.Type == "Expense").ToList();
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
            IncomePrMonth = new();
            List<Transaction> list = TransactionsForPeriod.Where(x => x.Type=="Income").ToList();
            ChartSeries series = new();
            double[] chartData = new double[12];
            for(int i=0; i < 12; i++)
            {
                List<Transaction> monthlyList = list.Where(x => DateTime.Parse(x.MadeAt).Month == i).ToList();
                double monthlySum = monthlyList.Sum(x => x.Value);
                //Console.WriteLine("$OverviewService.cs_IncomeForPeriod(): monthlysum for {0} is {1}",i,monthlySum);
                IncomePrMonth.Insert(i,monthlySum);
                // Divide income by 1000 to make chart appear nicer in UI
                chartData[i] = monthlySum/1000;
            }

            IncomeAndExpense.Add(new ChartSeries() { Name = "Income", Data = chartData });
        }
        private void ExpensesForPeriod()
        {
            ExpensesPrMonth = new();
            List<Transaction> list = TransactionsForPeriod.Where(x => x.Type == "Expense").ToList();
            double[] chartData = new double[12];
            for (int i = 0; i < 12; i++)
            {
                List<Transaction> monthlyList = list.Where(x => DateTime.Parse(x.MadeAt).Month == i).ToList();
                double monthlySum = monthlyList.Sum(x => x.Value);
                //Console.WriteLine("$OverviewService.cs_IncomeForPeriod(): monthlysum for {0} is {1}", i, monthlySum);
                ExpensesPrMonth.Insert(i, monthlySum*(-1));
                // Divide expenses by 1000 to make chart appear nicer in UI
                chartData[i] = (monthlySum / 1000)*(-1);
            }
            IncomeAndExpense.Add(new ChartSeries() { Name = "Expenses", Data = chartData });
        }
        private void ResultForPeriod()
        {
            ResultPrMonth = new();
            for(int i = 0; i< IncomePrMonth.ToArray().Length; i++)
            {   double result = IncomePrMonth.ToArray()[i] - ExpensesPrMonth.ToArray()[i];
                ResultPrMonth.Insert(i, result );
            }
        }
        private void ResultForPeriodAcc()
        {
            ResultPrMonthAcc = new();
            double[] resultPrMonth = ResultPrMonth.ToArray();
            for(int i=0; i< resultPrMonth.Length; i++)
            {
                if (i == 0)
                {
                    ResultPrMonthAcc.Insert(i, resultPrMonth[i]);
                }
                else
                {   if(DateTime.Now.Month >= i && DateTime.Now.Year >= YearSelected.Value.Year)
                    {
                        ResultPrMonthAcc.Insert(i, resultPrMonth[i] + ResultPrMonthAcc.ToArray()[i-1]);
                    }
                    else
                    {
                        ResultPrMonthAcc.Insert(i, 0);
                    }
                }
            }
        }
        private string CalculateSumForYear(List<double> list)
        {
            double sum = 0;
            foreach(double element in list)
            {
                sum += element;
            }
            int thousands = (int)sum/1000;
            int hundreds = (int)sum %1000;
            string result = thousands+"."+hundreds;

            return result;
        }
        private void GenerateResultsList()
        {
            Results = new();
            string[] months = GetMonths.ToArray();
            double[] incomeResults = IncomePrMonth.ToArray();
            double[] expensesResults = ExpensesPrMonth.ToArray();
            double[] monthlyResults = ResultPrMonth.ToArray() ;
            double[] monthlyResultAcc = ResultPrMonthAcc.ToArray() ;  
            for(int i = 0; i< incomeResults.Length; i++)
            {
                Results.Insert(i,
                    new Result
                    {
                        Month = months[i],
                        Income = incomeResults[i],
                        Expenses = expensesResults[i],
                        MonthResult=monthlyResults[i],
                        MonthResultAcc=monthlyResultAcc[i]
                    });
            }
        }
        
    }
}





