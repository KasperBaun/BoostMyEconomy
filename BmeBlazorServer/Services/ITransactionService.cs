using BmeModels;
using MudBlazor;

namespace BmeBlazorServer.Services
{
    public interface ITransactionService
    {
        
        event Action OnChange;
        List<Transaction> UserTransactions { get; set; }
        Task<List<Transaction>> FetchUserTransactionsFromAPI();
        Task<List<Transaction>> GetAllUserTransactions();
        DateRange GetDateRange();
        void SetDateRange(DateRange dateRange);


    }
}
