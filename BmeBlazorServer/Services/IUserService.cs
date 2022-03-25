using BmeModels;

namespace BmeBlazorServer.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();

        Task<HttpResponseMessage> RegisterUser(UserRegistrationDTO user);

        Task<HttpResponseMessage> UpdateUser(User user);

        Task<HttpResponseMessage> DeleteUser(int userId);

    }
}
