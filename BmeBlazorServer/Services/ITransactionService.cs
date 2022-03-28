using BmeModels;
using MudBlazor;

namespace BmeBlazorServer.Services
{
    public interface ITransactionService
    {
        
        int Balance { get; set; }
        event Action OnChange;
        List<Transaction> UserTransactionsForPeriod { get; set; }
        List<Transaction> IncomeTransactionsForPeriod { get; set; }
        List<Transaction> ExpenseTransactionsForPeriod { get; set; }
        Task<List<Transaction>> FetchUserTransactionsFromAPI();
        Task<bool> GetAllUserTransactions();
        DateRange DateRange { get; set; }
        void PeriodChanged();


    }
}
