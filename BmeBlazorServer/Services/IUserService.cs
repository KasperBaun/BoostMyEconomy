using BmeModels;

namespace BmeBlazorServer.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();

        Task<HttpResponseMessage> RegisterUser(User user);

        Task<HttpResponseMessage> UpdateUser(User user);

        Task<HttpResponseMessage> DeleteUser(int userId);

    }
}
