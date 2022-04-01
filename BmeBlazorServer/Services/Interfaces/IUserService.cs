using BmeModels;

namespace BmeBlazorServer.Services
{
    public interface IUserService
    {
        event Action? OnChange;
        public User CurrentUser { get; set; }
        Task<bool> FetchCurrentUser();
        Task<List<User>> GetUsers();
        Task<HttpResponseMessage> UpdateUser(User user);
        Task<HttpResponseMessage> DeleteUser(int userId);

    }
}
