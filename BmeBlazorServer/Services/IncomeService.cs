﻿using BmeModels;
using MudBlazor;
using BmeBlazorServer.Repositories;

namespace BmeBlazorServer.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly ITransactionRepository transactionRepository;
        private List<Transaction> UserTransactions { get; set; } = new();
        public DateRange? PeriodSelected { get; set; }
        public List<Transaction> IncomeForPeriod { get; set; } = new();
        public ChartData IncomeSourcesForPeriod { get; set; } = new();
        public List<ChartSeries> IncomeHistory { get; set; } = new();
        public string[] IncomeHistoryLabels { get; set; } = Array.Empty<string>();
        public event Action? OnChange;
        public IncomeService(ITransactionRepository _transactionRepository)
        {
            transactionRepository = _transactionRepository;
            PeriodSelected = new DateRange(new DateTime(DateTime.Now.Year, 1, 1), DateTime.Now.Date);
        }
        private List<Transaction> FilterTransactionsFromSelectedPeriod(DateRange periodSelected)
        {
            List<Transaction> list = new();
            if(periodSelected.Start.HasValue && periodSelected.End.HasValue)
            {
                DateTime Start = periodSelected.Start.Value;
                DateTime End = periodSelected.End.Value;
                list = UserTransactions.Where(x => 
                x.MadeAt.Date >= Start && x.MadeAt <= End && x.Type=="Income").ToList();
                return list;
            }
            else
            {
                return list;
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
                Console.WriteLine("Month: {0}\n", t.MadeAt.Month);
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
        public async Task<bool> InitializeService()
        {
            UserTransactions = await transactionRepository.GetTransactions();
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
