using BmeModels;

namespace BmeBlazorServer.Services
{
    public class TransactionService : ITransactionService
    {
        public event Action? OnChange;

        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
     

        public TransactionService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
        }

        public Task<bool> CreateTransaction(TransactionDTO transaction)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTransaction(TransactionDTO transaction)
        {
            throw new NotImplementedException();
        }

        public Task<List<Transaction>> GetTransactions()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateTransaction(TransactionDTO transaction)
        {
            throw new NotImplementedException();
        }
    }
}
