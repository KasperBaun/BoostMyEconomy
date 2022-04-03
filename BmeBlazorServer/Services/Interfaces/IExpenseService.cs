using BmeModels;
using MudBlazor;

namespace BmeBlazorServer.Services
{
    public interface IExpenseService
    {
        event Action? OnChange;
        List<Transaction> ExpensesForPeriod { get; set; }
        ChartData ExpenseSourcesForPeriod { get; set; }
        List<ChartSeries> ExpenseHistory { get; set; }
        string[] ExpenseHistoryLabels { get; set; }
        Task<bool> InitializeService();
        DateRange? PeriodSelected { get; set; }
        void PeriodChanged();
    }
}
