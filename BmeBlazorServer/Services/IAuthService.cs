using BmeModels;

namespace BmeBlazorServer.Services
{
    public interface IAuthService
    {
        Task<HttpResponseMessage> RegisterUser(UserRegistrationDTO user);

        Task<HttpResponseMessage> Login(UserLoginDTO user);

    }
}
