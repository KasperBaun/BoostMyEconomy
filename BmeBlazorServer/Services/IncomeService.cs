using BmeModels;

namespace BmeBlazorServer.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        public IncomeService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
        }
        public DateTime? PeriodSelected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event Action OnChange;

        public Task<bool> AddIncomeTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<List<Transaction>> GetAllIncomeTransactions()
        {
            throw new NotImplementedException();
        }

        public Task<bool> InitializeService()
        {
            throw new NotImplementedException();
        }

        public void PeriodChanged()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveIncomeTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
