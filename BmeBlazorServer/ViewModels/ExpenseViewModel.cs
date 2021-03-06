using BmeModels;
using MudBlazor;
using BmeBlazorServer.Repositories;

namespace BmeBlazorServer.Services
{
    public class ExpenseViewModel : IExpenseViewModel
    {
        private readonly ITransactionRepository transactionRepository;
        private List<Transaction> UserTransactions { get; set; } = new();
        public DateRange? PeriodSelected { get; set; }
        public List<Transaction> ExpensesForPeriod { get; set; } = new();
        public ChartData ExpenseCategoriesForPeriod { get; set; } = new();
        public List<ChartSeries> ExpenseHistory { get; set; } = new();
        public string[] ExpenseHistoryLabels { get; set; } = Array.Empty<string>();
        public List<TableItem> VarExpenseTableItems { get; set; } = new();
        public List<TableItem> FixedExpenseTableItems { get; set; } = new();
        public event Action? OnChange;
        public ExpenseViewModel(ITransactionRepository _transactionRepository)
        {
            transactionRepository = _transactionRepository;
            PeriodSelected = new DateRange(new DateTime(DateTime.Now.Year, 1, 1), DateTime.Now.Date);
        }
        public async Task<bool> InitializeService()
        {
            UserTransactions = await transactionRepository.GetTransactions();
            if (!PeriodSelected!.Start.HasValue || !PeriodSelected.End.HasValue)
            {
                throw new Exception("Error with value from PeriodSelected@IncomeService.cs");
            }
            else
            {
                ExpensesForPeriod = FilterTransactionsFromSelectedPeriod(new DateRange(PeriodSelected.Start.Value, PeriodSelected.End.Value));
            }
            ExpenseCategoriesForPeriod = FilterCategories(ExpensesForPeriod);
            FilterHistory(ExpensesForPeriod);
            FilterVarFixedExpenses(ExpensesForPeriod);
            OnChange?.Invoke();
            return true;
        }
        public async void PeriodChanged()
        {
            ClearData();
            await InitializeService();
            OnChange?.Invoke();
        }
        private void ClearData()
        {
            UserTransactions.Clear();
            ExpensesForPeriod.Clear();
            ExpenseHistory.Clear();
            ExpenseCategoriesForPeriod = new();
            ExpenseHistoryLabels = Array.Empty<string>();
            VarExpenseTableItems.Clear();
            FixedExpenseTableItems.Clear();
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
        private static ChartData FilterCategories(List<Transaction> expenseTransactions)
        {
            ChartData chartData = new();
            List<double> data = new();
            List<string> transactionCategories = new();
            foreach(Transaction t in expenseTransactions)
            {
                if (transactionCategories.Contains(t.Category.Title))
                {
                    int index = transactionCategories.FindIndex(c => c == t.Category.Title);
                    double sourceSum = expenseTransactions.Where(x => x.Category.Title == t.Category.Title).Sum(y => y.Value);
                    data[index] = sourceSum;
                }
                else
                {
                    transactionCategories.Add(t.Category.Title);
                    int index = transactionCategories.FindIndex(c => c == t.Category.Title);
                    data.Insert(index, t.Value*(-1));
                }
            }
            double total = data.Sum();
            List<double> temp = data.OrderByDescending(sum => sum).ToList();
            temp.Reverse();
            List<string> orderedCategories = new();
            foreach (double d in temp)
            {
                int index = data.FindIndex(c => c == d);
                orderedCategories.Add(transactionCategories[index]);
            }
            for (int i = 0; i < temp.Count; i++)
            {
                temp[i] = temp[i] / total;
                temp[i] = Math.Round(temp[i], 3);
            }
            chartData.Labels = orderedCategories.Take(10).ToArray();
            chartData.Data = temp.Take(10).ToArray();
            /*
            for (int i = 0; i < data.Count; i++)
            {
                Console.WriteLine("Data: {0}, Label: {1}", data[i], transactionCategories[i]);
            }
            Console.WriteLine("\n");
            for (int i=0; i < chartData.Data.Length; i++)
            {
                Console.WriteLine("Data: {0}, Label: {1}", chartData.Data[i], chartData.Labels[i] );
            }
            */

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
                    data.Insert(index,sourceSum*(-1));
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
        private void FilterVarFixedExpenses(List<Transaction> expensesList)
        {
            List<double> sum = new();
            List<string> sumCategories = new();
            foreach (Transaction t in expensesList)
            {
                if (sumCategories.Contains(t.Category.Title))
                {
                    int index = sumCategories.FindIndex(c => c == t.Category.Title);
                    double sourceSum = expensesList.Where(x => x.Category.Title == t.Category.Title).Sum(y => y.Value);
                    sum[index] = sourceSum;
                }
                else
                {
                    sumCategories.Add(t.Category.Title);
                    int index = sumCategories.FindIndex(c => c == t.Category.Title);
                    sum.Insert(index, t.Value);
                }
            }
            foreach(double d in sum)
            {
                Math.Round(d, 2);
            }
            List<TableItem> varItems = new();
            List<int> varIds = new() { 19, 21, 22, 23, 26, 29, 30 };
            List<TableItem> fixedItems = new();
            List<int> fixedIds = new() { 15, 16, 17, 18, 20, 25, 27, 28 };
            foreach (string category in sumCategories)
            {
                Category cat = expensesList.Find(c => c.Category.Title == category)!.Category;
                int index = sumCategories.FindIndex(c => c == category);
                TableItem item = new();
                item.Name = category;
                item.Value = sum[index];
                item.IconString = CategoryToIcon(cat.Id);
                if (varIds.Contains(cat.Id)){
                    varItems.Add(item);
                }
                if (fixedIds.Contains(cat.Id))
                {
                    fixedItems.Add(item);
                }
            }
            varItems.Sort((a, b) =>
                    a.Value
                    .CompareTo(
                    b.Value
                    ));
            fixedItems.Sort((a, b) =>
                    a.Value
                    .CompareTo(
                    b.Value
                    ));

            VarExpenseTableItems = varItems;
            FixedExpenseTableItems= fixedItems;
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
        private static string CategoryToIcon(int categoryId)
        {
            return categoryId switch
            {
                /* 0-14 is income categories */
                0 =>  Icons.Material.Filled.Work,
                1 =>  Icons.Material.Filled.MonetizationOn,
                2 =>  Icons.Material.Rounded.Payments,
                3 =>  Icons.Material.Filled.Business,
                4 =>  Icons.Material.Rounded.ChildCare,
                5 =>  Icons.Material.Filled.Balance,
                6 =>  Icons.Material.Filled.PriceChange,
                7 =>  Icons.Material.Filled.AttachMoney,
                8 =>  Icons.Material.Filled.AreaChart,
                9 =>  Icons.Material.Filled.AreaChart,
                10 => Icons.Material.Filled.CardGiftcard,
                11 => Icons.Material.Filled.ShoppingCart,
                12 => Icons.Material.Filled.Elderly,
                13 => Icons.Material.Filled.WaterfallChart,
                14 => Icons.Material.Filled.Chair,

                /* 15-30 is expense categories */
                15 => Icons.Material.Filled.HomeWork,
                16 => Icons.Material.Filled.Power,
                17 => Icons.Material.Rounded.EmojiTransportation,
                18 => Icons.Material.Filled.Policy,
                19 => Icons.Material.Filled.ChildFriendly,
                20 => Icons.Material.Filled.CurrencyExchange,
                21 => Icons.Material.Filled.AddShoppingCart,
                22 => Icons.Material.Filled.Theaters,
                23 => Icons.Material.Filled.Restaurant,
                24 => Icons.Material.Filled.CrisisAlert,
                25 => Icons.Material.Filled.Subscriptions,
                26 => Icons.Material.Filled.EscalatorWarning,
                27 => Icons.Material.Filled.MedicalServices,
                28 => Icons.Material.Filled.Handyman,
                29 => Icons.Material.Filled.Pets,
                30 => Icons.Material.Filled.Engineering,
                _ => string.Empty,
            };
        }
    }
}
