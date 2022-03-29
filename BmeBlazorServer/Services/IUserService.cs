using BmeModels;

namespace BmeBlazorServer.Services
{
    public interface IUserService
    {
        Task<User> GetCurrentUser();
        string UserName { get; set; }
        event Action OnChange;
        void ParseLoggedInUserName();
        Task<List<User>> GetUsers();
        Task<HttpResponseMessage> UpdateUser(User user);
        Task<HttpResponseMessage> DeleteUser(int userId);

    }
}
