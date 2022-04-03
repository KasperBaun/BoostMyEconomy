using BmeModels;

namespace BmeBlazorServer.Repositories
{
    public interface IUserRepository
    {
        event Action? OnChange;
        public User CurrentUser { get; set; }
        public string LastLogin { get; set; }
        Task<User> GetCurrentUser();
        Task<List<User>> GetUsers();
        Task<HttpResponseMessage> UpdateUser(User user);
        Task<HttpResponseMessage> DeleteUser(int userId);

    }
}
