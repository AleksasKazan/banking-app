using System.Threading.Tasks;
using Contracts.Models.Request;
using Domain.Clients.Firebase.Models;

namespace Domain.Clients.Firebase
{
    public interface IFireBaseClient
    {
        Task<SignUpResponse> SignUp(UserSignUpRequestModel user);

        Task<SignInResponse> SignIn(UserSignInRequestModel user);

        Task<ErrorResponse> DeleteAccount(string idToken);

        Task<PasswordResetResponse> PasswordReset(PasswordResetRequestModel user);

        Task<ChangeEmailResponse> ChangeEmail(string email, string idToken);

        Task<ChangePasswordResponse> ChangePassword(string newPssword, string idToken);
    }
}
