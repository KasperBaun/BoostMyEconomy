using BmeModels;

namespace BmeBlazorServer.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetAllUserTransactions();

    }
}
