using BmeModels;

namespace BmeBlazorServer.Services;

public interface ITransactionService
{
    event Action? OnChange;
    Task<List<Transaction>> GetTransactions();
    Task<ResponseModel> CreateTransaction(TransactionDTO transaction);
    Task<ResponseModel> UpdateTransaction(Transaction transaction);
    Task<ResponseModel> DeleteTransaction(Transaction transaction);

}
