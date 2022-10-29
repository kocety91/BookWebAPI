using BookWebAPI.Dtos.Identity;

namespace BookWebAPI.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResponseModel> RegisterAsync(RegisterRequestModel model);

        Task<AuthenticationResponseModel> LoginAsync(LoginRequestModel model);
    }
}
