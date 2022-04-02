using BmeModels;

namespace BmeBlazorServer.Repositories
{
    public interface IUserRepository
    {
        event Action? OnChange;
        public User CurrentUser { get; set; }
        Task<bool> FetchCurrentUser();
        Task<List<User>> GetUsers();
        Task<HttpResponseMessage> UpdateUser(User user);
        Task<HttpResponseMessage> DeleteUser(int userId);

    }
}
