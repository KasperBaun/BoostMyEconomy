using BmeModels;
using MudBlazor;
using BmeBlazorServer.Repositories;
namespace BmeBlazorServer.Services
{
    public class OverviewService : IOverviewService
    {
        private readonly ITransactionRepository transactionRepository;
        private List<Transaction> UserTransactions { get; set; } = new();
        public DateTime? YearSelected { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public List<Transaction> TransactionsForPeriod { get; set; } = new List<Transaction>();
        public List<double> IncomePrMonth { get; set; } = new();
        public List<double> ExpensesPrMonth { get; set; } = new();
        public List<double> ResultPrMonth { get; set; } = new();
        public List<double> ResultPrMonthAcc { get; set; } = new();
        public List <Result> Results { get; set; } = new();
        public List<ChartSeries> IncomeAndExpense { get; set; } = new();
        public ChartData ExpenseSourcesForPeriod { get; set; } = new();
        public List<string> GetMonths { get; set; } = new(){ "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public string SumIncome { get; set; } = string.Empty;
        public string NetIncome { get; set; } = string.Empty;
        public int Balance { get; set; } = 1;
        public event Action? OnChange;

        public OverviewService(ITransactionRepository _transactionRepository)
        {
           transactionRepository = _transactionRepository;
        }
             
        public async Task<bool> InitializeService()
        {
     
            if(!UserTransactions.Any())
            {
                UserTransactions = await transactionRepository.GetTransactions();
                //await FetchUserTransactions();
            }
            if(YearSelected == null)
            {
                Console.WriteLine("$OverviewService.cs@InitializeService(): YearSelected is null!");
            }
            else
            {
                TransactionsForPeriod = FilterTransactionsFromSelectedYear(YearSelected.Value);
            }
            CalculateBalanceForPeriod();
            IncomeForPeriod();
            SumIncome = CalculateSumForYear(IncomePrMonth);
            ExpensesForPeriod();
            ResultForPeriod();
            ResultForPeriodAcc();
            GenerateResultsList();
            ExpenseSourcesForPeriod = FilterSources(TransactionsForPeriod);
            OnChange?.Invoke();
            return true;
        }
        private List<Transaction> FilterTransactionsFromSelectedYear(DateTime yearSelected)
        {
            List<Transaction> list = new();
            list = UserTransactions.Where(x =>  x.MadeAt.Year == yearSelected.Year).ToList();
            list.Sort((x, y) => DateTime.Compare(x.MadeAt, y.MadeAt));
            list.Reverse();
            OnChange?.Invoke();
            return list;
        }
        public async void PeriodChanged()
        {
            await InitializeService();
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
            NetIncome = (income - expenses).ToString();
          
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
            IncomeAndExpense = new();
            List<Transaction> list = TransactionsForPeriod.Where(x => x.Type=="Income").ToList();
            ChartSeries series = new();
            double[] chartData = new double[12];
            for(int i=0; i < 12; i++)
            {
                List<Transaction> monthlyList = list.Where(x => x.MadeAt.Month == i).ToList();
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
                List<Transaction> monthlyList = list.Where(x => x.MadeAt.Month == i).ToList();
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
                {   if(YearSelected == null)
                    {
                        Console.WriteLine("$OverviewService@ResultForPeriodAcc(): YearSelected = null!");
                    }
                    else
                    {
                        if(DateTime.Now.Month >= i && DateTime.Now.Year >= YearSelected.Value.Year)
                        {
                            double prevSum = ResultPrMonthAcc.ToArray()[i-1];
                            if(prevSum == 0)
                            {
                                for (int x = resultPrMonth.Length; x >= 0;)
                                {
                                    x--;
                                    if(resultPrMonth[x] > prevSum)
                                        prevSum = ResultPrMonthAcc.ToArray()[x]; break;
                                }
                            }
                            ResultPrMonthAcc.Insert(i, resultPrMonth[i] + prevSum);
                        }
                        else
                        {
                            ResultPrMonthAcc.Insert(i, 0);
                        }
                    }
                }
            }
        }
        private static string CalculateSumForYear(List<double> list)
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
        public string CategoryToIcon(int categoryId)
        {
            return categoryId switch
            {
                /* 0-14 is income categories */
                0 => Icons.Material.Filled.Work,
                1 => Icons.Material.Filled.MonetizationOn,
                2 => Icons.Material.Rounded.Payments,
                3 => Icons.Material.Filled.Business,
                4 => Icons.Material.Rounded.ChildCare,
                5 => Icons.Material.Filled.Balance,
                6 => Icons.Material.Filled.PriceChange,
                7 => Icons.Material.Filled.AttachMoney,
                8 => Icons.Material.Filled.AreaChart,
                9 => Icons.Material.Filled.AreaChart,
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
        private static ChartData FilterSources(List<Transaction> expenseTransactions)
        {
            List<double> data = new();
            List<string> transactionCategories = new();
            foreach (Transaction t in expenseTransactions)
            {
                if (t.Type == "Expense")
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
                        data.Insert(index, t.Value * (-1));
                    }
                }
            }

            // Test
            //Console.WriteLine("$Incomeservice.cs@FilterSources() - double[] Data.length: {0}  string[] Labels.length: {1}\n", data.ToArray().Length, transactionCategories.ToArray().Length);

            ChartData chartData = new()
            {
                Data = data.ToArray(),
                Labels = transactionCategories.ToArray()
            };
            return chartData;
        }
    }
}





