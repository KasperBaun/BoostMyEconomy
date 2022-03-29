using BmeModels;

namespace BmeBlazorServer.Services
{
    public interface IIncomeService
    {
        event Action OnChange;
        Task<bool> AddIncomeTransaction(Transaction transaction);
        Task<bool> RemoveIncomeTransaction(Transaction transaction);
        Task<List<Transaction>> GetAllIncomeTransactions();
        Task<bool> InitializeService();
        DateTime? PeriodSelected { get; set; }
        void PeriodChanged();

    }
}
