using BmeModels;
using MudBlazor;

namespace BmeBlazorServer.Services
{
    public interface IOverviewService
    {
        public string SumIncome { get; set; }
        int Balance { get; set; }
        event Action OnChange;
        List<Transaction> TransactionsForPeriod { get; set; }
        List<double> IncomePrMonth { get; set; }
        List<double> ExpensesPrMonth { get; set; }
        List<double> ResultPrMonth { get; set; }
        List<double> ResultPrMonthAcc { get; set; }
        List<string> GetMonths { get; set; }
        List<ChartSeries> IncomeAndExpense { get; set; }
        Task<bool> InitializeOverviewService();
        DateTime? YearSelected { get; set; }
        void PeriodChanged();


    }
}
