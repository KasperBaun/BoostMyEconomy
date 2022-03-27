using BmeModels;
using MudBlazor;

namespace BmeBlazorServer.Services
{
    public interface ITransactionService
    {
        
        event Action OnChange;
        List<Transaction> UserTransactions { get; set; }
        Task<List<Transaction>> FetchUserTransactionsFromAPI();
        Task<bool> GetAllUserTransactions();
        DateRange DateRange { get; set; }
        void DateRangeChanged();


    }
}
