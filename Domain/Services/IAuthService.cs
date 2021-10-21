using System;
using System.Threading.Tasks;
using Contracts.Models.Request;
using Contracts.Models.Response;
using Domain.Clients.Firebase.Models;

namespace Domain.Services
{
    public interface IAuthService
    {
        Task<UserSignUpResponseModel> SignUpAsync(UserSignUpRequestModel request);

        Task<UserSignInResponseModel> SignInAsync(UserSignInRequestModel request);

        Task<SuccessResponse> DeleteAccount(UserSignInRequestModel request);

        Task<PasswordResetResponseModel> PasswordReset(PasswordResetRequestModel request);

        Task<ChangeEmailResponseModel> ChangeEmail(UserSignInRequestModel request, string newEmail);

        Task<ChangePasswordResponseModel> ChangePassword(UserSignInRequestModel request, string newPassword);
    }
}
