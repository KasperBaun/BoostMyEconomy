using BmeModels;
using MudBlazor;

namespace BmeBlazorServer.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorageService;
        private List<Transaction> AllUserTransactions { get; set; }
        public List<Transaction> IncomeForPeriod { get; set; } = new List<Transaction>();
        public IncomeService(HttpClient _httpClient, ILocalStorageService _localStorageService)
        {
            httpClient = _httpClient;
            localStorageService = _localStorageService;
        }
        public DateRange? PeriodSelected { get; set; } = new DateRange(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.Date);

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
            InitializeService();
            OnChange?.Invoke();
        }

        public Task<bool> RemoveIncomeTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
