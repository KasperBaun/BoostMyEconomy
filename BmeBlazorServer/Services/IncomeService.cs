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
        public List<Transaction> IncomeForPeriod { get; set; } = new();
        public ChartData IncomeSourcesForPeriod { get; set; } = new();
        public List<ChartSeries> IncomeHistory { get; set; } = new();
        public string[] IncomeHistoryLabels { get; set; } = Array.Empty<string>();
        public event Action? OnChange;
        public IncomeService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
            PeriodSelected = new DateRange(new DateTime(DateTime.Now.Year, 1, 1), DateTime.Now.Date);
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
        private ChartData FilterSources(List<Transaction> incomeTransactions)
        {
            List<double> data = new();
            List<string> transactionCategories = new();
            foreach(Transaction t in incomeTransactions)
            {
                if (transactionCategories.Contains(t.Source))
                {
                    int index = transactionCategories.FindIndex(c => c == t.Source);
                    double sourceSum = incomeTransactions.Where(x => x.Source == t.Source).Sum(y => y.Value);
                    data[index] = sourceSum;
                }
                else
                {
                    transactionCategories.Add(t.Source);
                    int index = transactionCategories.FindIndex(c => c == t.Source);
                    data.Insert(index, t.Value);
                }
            }

            // Test
            //Console.WriteLine("$Incomeservice.cs@FilterSources() - double[] Data.length: {0}  string[] Labels.length: {1}\n", data.ToArray().Length, transactionCategories.ToArray().Length);

            ChartData chartData = new(){
                Data = data.ToArray(),
                Labels = transactionCategories.ToArray()
            };
            return chartData;
        }
        private void FilterHistory(List<Transaction> incomeTransactions)
        {
            List<double> data = new();
            List<string> months = new();

            foreach (Transaction t in incomeTransactions)
            {
                int tMonth = DateTime.Parse(t.MadeAt).Month;
                string tMonthConverted = ConvertMonthToString(tMonth);
                if (months.Contains(tMonthConverted)){
                    continue;
                }
                else
                {
                    months.Add(tMonthConverted);
                    int index = months.FindIndex(m => m == tMonthConverted);
                    double sourceSum = incomeTransactions.Where(x => 
                        DateTime.Parse(x.MadeAt).Month == tMonth).ToList().Sum(y => y.Value);
                    data.Insert(index,sourceSum);
                }
            }

            // Test
            Console.WriteLine("$Incomeservice.cs@FilterHistory() - double[] Data.length: {0}  string[] Months.length: {1}\n", data.ToArray().Length, months.ToArray().Length);
            /*
             * Dirty-fix required because the chart needs atleast 4 x-points and 4-y points 
             * should not hit this edge case very often, only when data-representation is very low
            */
            if (data.ToList().Count < 4 && months.ToList().Count < 4)
            {
               int lengthOfList = data.ToList().Count;
               for(int i=lengthOfList; i<4; i++)
                {
                    data.Add(0);
                    months.Add("");
                }
            }
            
            List<ChartSeries> series = new();
            series.Add(new ChartSeries
            {
                Data = data.ToArray(),
                Name = "Income history"
            });
            IncomeHistory = series;
            IncomeHistoryLabels = months.ToArray();
            return;
        }
        private static string ConvertMonthToString(int month)
        {
            if(month == 0)
            {
                return String.Empty;
            }
            else
            {
                switch (month)
                {
                    default:    return String.Empty;
                    case 1:     return "Jan";
                    case 2:     return "Feb";
                    case 3:     return "Mar";
                    case 4:     return "Apr";
                    case 5:     return "May";
                    case 6:     return "Jun";
                    case 7:     return "Jul";
                    case 8:     return "Aug";
                    case 9:     return "Sep";
                    case 10:    return "Oct";
                    case 11:    return "Nov";
                    case 12:    return "Dec";
                }
            }
            
        }
        public async Task<bool> InitializeService()
        {
            await FetchCategories();
            await FetchUserTransactions();
            IncomeForPeriod = FilterTransactionsFromSelectedPeriod(PeriodSelected);
            IncomeSourcesForPeriod = FilterSources(IncomeForPeriod);
            FilterHistory(IncomeForPeriod);
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
