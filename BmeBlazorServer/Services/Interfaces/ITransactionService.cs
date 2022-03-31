using BmeModels;

namespace BmeBlazorServer.Services;

public interface ITransactionService
{
    event Action? OnChange;
    Task<List<Transaction>> GetTransactions();
    Task<bool> CreateTransaction(TransactionDTO transaction);
    Task<bool> UpdateTransaction(TransactionDTO transaction);
    Task<bool> DeleteTransaction(TransactionDTO transaction);

}
