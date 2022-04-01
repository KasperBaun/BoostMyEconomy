using BmeModels;
using MudBlazor;

namespace BmeBlazorServer.Services
{
    public interface IIncomeService
    {
        event Action? OnChange;
        List<Transaction> IncomeForPeriod { get; set; }
        ChartData IncomeSourcesForPeriod { get; set; }
        List<ChartSeries> IncomeHistory { get; set; }
        string[] IncomeHistoryLabels { get; set; }
        Task<bool> AddIncomeTransaction(Transaction transaction);
        Task<bool> RemoveIncomeTransaction(Transaction transaction);
        Task<bool> InitializeService();
        DateRange? PeriodSelected { get; set; }
        void PeriodChanged();

    }
}
