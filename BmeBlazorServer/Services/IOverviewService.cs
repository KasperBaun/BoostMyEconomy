using BmeModels;
using MudBlazor;

namespace BmeBlazorServer.Services
{
    public interface IOverviewService
    {
        public string SumIncomeForYear { get; set; }
        int Balance { get; set; }
        event Action OnChange;
        List<Transaction> UserTransactionsForPeriod { get; set; }
        List<ChartSeries> IncomeForYear { get; set; }
        List<Transaction> ExpenseTransactionsForPeriod { get; set; }
        Task<bool> InitializeOverviewService();
        DateTime? YearSelected { get; set; }
        void PeriodChanged();


    }
}
