using BmeModels;
using MudBlazor;

namespace BmeBlazorServer.Services
{
    public interface IIncomeService
    {
        event Action? OnChange;
        List<Transaction> IncomeForPeriod { get; set; }
        List<ChartData> IncomeSourcesForPeriod { get; set; }
        Task<bool> AddIncomeTransaction(Transaction transaction);
        Task<bool> RemoveIncomeTransaction(Transaction transaction);
        Task<bool> InitializeService();
        DateRange? PeriodSelected { get; set; }
        void PeriodChanged();

    }
}
