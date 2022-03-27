using BmeModels;

namespace BmeBlazorServer.Services
{
    public interface IUserService
    {
        string UserName { get; set; }
        event Action OnChange;
        void ParseLoggedInUserName();
        Task<List<User>> GetUsers();
        Task<HttpResponseMessage> UpdateUser(User user);
        Task<HttpResponseMessage> DeleteUser(int userId);

    }
}
