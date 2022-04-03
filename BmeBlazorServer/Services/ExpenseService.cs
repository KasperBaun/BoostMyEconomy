using BmeModels;
using MudBlazor;
using BmeBlazorServer.Repositories;

namespace BmeBlazorServer.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ITransactionRepository transactionRepository;
        private List<Transaction> UserTransactions { get; set; } = new();
        public DateRange? PeriodSelected { get; set; }
        public List<Transaction> ExpensesForPeriod { get; set; } = new();
        public ChartData ExpenseSourcesForPeriod { get; set; } = new();
        public List<ChartSeries> ExpenseHistory { get; set; } = new();
        public string[] ExpenseHistoryLabels { get; set; } = Array.Empty<string>();
        public event Action? OnChange;
        public ExpenseService(ITransactionRepository _transactionRepository)
        {
            transactionRepository = _transactionRepository;
            PeriodSelected = new DateRange(new DateTime(DateTime.Now.Year, 1, 1), DateTime.Now.Date);
        }
        public async Task<bool> InitializeService()
        {
            UserTransactions.Clear();
            UserTransactions = await transactionRepository.GetTransactions();
            if(PeriodSelected.Start.HasValue && PeriodSelected.End.HasValue)
            {
                ExpensesForPeriod = FilterTransactionsFromSelectedPeriod(new DateRange(PeriodSelected.Start.Value,PeriodSelected.End.Value));
            }
            else
            {
                throw new Exception("Error with value from PeriodSelected@IncomeService.cs");
            }
            ExpenseSourcesForPeriod = FilterSources(ExpensesForPeriod);
            FilterHistory(ExpensesForPeriod);
            OnChange?.Invoke();
            return true;
        }
        public async void PeriodChanged()
        {
            await InitializeService();
            OnChange?.Invoke();
        }
      
        private List<Transaction> FilterTransactionsFromSelectedPeriod(DateRange periodSelected)
        {
            List<Transaction> list = new();
            if(periodSelected.Start.HasValue && periodSelected.End.HasValue)
            {
                var Start = DateOnly.FromDateTime(periodSelected.Start.Value);
                var End = DateOnly.FromDateTime(periodSelected.End.Value);
                list = UserTransactions.Where(x => DateOnly.FromDateTime(x.MadeAt) >= Start && DateOnly.FromDateTime(x.MadeAt) <= End && x.Type=="Expense").ToList();
                return list;
            }
            else
            {
                throw new Exception("Error with periodSelected.Value @ IncomeService.cs - FilterTransactionsFromSelectedPeriod()");
            }
        }
        private static ChartData FilterSources(List<Transaction> incomeTransactions)
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
            incomeTransactions.Sort((a, b) =>
                    a.MadeAt.Month
                    .CompareTo(
                    b.MadeAt.Month
                    ));

            foreach (Transaction t in incomeTransactions)
            {
                //Console.WriteLine("Month: {0}\n", t.MadeAt.Month);
                int tMonth = t.MadeAt.Month;
                string tMonthConverted = ConvertMonthToString(tMonth);
                if (months.Contains(tMonthConverted)){
                    continue;
                }
                else
                {
                    months.Add(tMonthConverted);
                    int index = months.FindIndex(m => m == tMonthConverted);
                    double sourceSum = incomeTransactions.Where(x => 
                        x.MadeAt.Month == tMonth).ToList().Sum(y => y.Value);
                    data.Insert(index,sourceSum);
                }
            }

            // Test
            Console.WriteLine("$ExpenseService.cs@FilterHistory() - double[] Data.length: {0}  string[] Months.length: {1}\n", data.ToArray().Length, months.ToArray().Length);
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
            ExpenseHistory = series;
            ExpenseHistoryLabels = months.ToArray();
            return;
        }
        private static string ConvertMonthToString(int month)
        {
            return month switch
            {
                1 => "Jan",
                2 => "Feb",
                3 => "Mar",
                4 => "Apr",
                5 => "May",
                6 => "Jun",
                7 => "Jul",
                8 => "Aug",
                9 => "Sep",
                10 => "Oct",
                11 => "Nov",
                12 => "Dec",
                _ => String.Empty,
            };
        }
    }
}
