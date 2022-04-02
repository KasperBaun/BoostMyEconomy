using BmeModels;

namespace BmeBlazorServer.Repositories
{
    public interface ITransactionRepository
    {
        event Action? OnChange;
        Task<List<Transaction>> GetTransactions();
        Task<ResponseModel> CreateTransaction(TransactionDTO transaction);
        Task<ResponseModel> UpdateTransaction(Transaction transaction);
        Task<ResponseModel> DeleteTransaction(Transaction transaction);
    }
}
