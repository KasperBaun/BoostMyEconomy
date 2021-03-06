using BmeModels;
using MudBlazor;

namespace BmeBlazorServer.Services
{
    public interface IIncomeViewModel
    {
        event Action? OnChange;
        List<Transaction> IncomeForPeriod { get; set; }
        ChartData IncomeSourcesForPeriod { get; set; }
        List<ChartSeries> IncomeHistory { get; set; }
        public List<TableItem> IncomeSourceTableItems { get; set; }
        public List<TableItem> IncomeCategoryTableItems { get; set; }
        string[] IncomeHistoryLabels { get; set; }
        Task<bool> InitializeService();
        DateRange? PeriodSelected { get; set; }
        void PeriodChanged();
    }
}
