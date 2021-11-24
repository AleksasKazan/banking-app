using System.Threading.Tasks;
using Contracts.Models.Request;
using Contracts.Models.Response;
using Domain.Exceptions;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("signUp")]
        public async Task<ActionResult<UserSignUpResponseModel>> SignUp(UserSignUpRequestModel request)
        {
            try
            {
                var response = await _authService.SignUpAsync(request);

                return response;
            }

            catch (FirebaseException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("signIn")]
        public async Task<ActionResult<UserSignInResponseModel>> SignIn(UserSignInRequestModel request)
        {
            try
            {
                var response = await _authService.SignInAsync(request);

                return response;
            }

            catch (FirebaseException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("deleteAccount")]
        public async Task<ActionResult<SuccessResponse>> DeleteAccount(UserSignInRequestModel request)
        {
            try
            {
                var response = await _authService.DeleteAccount(request);

                return response;
            }

            catch (FirebaseException e)
            {
                return BadRequest(e.Message);
            }            
        }

        [HttpPost]
        [Route("passwordReset")]
        public async Task<ActionResult<PasswordResetResponseModel>> PasswordReset(PasswordResetRequestModel request)
        {
            try
            {
                var response = await _authService.PasswordReset(request);

                return response;
            }

            catch (FirebaseException e)
            {
                return BadRequest(e.Message);
            }           
        }

        [HttpPost]
        [Route("changeEmail")]
        public async Task<ActionResult<ChangeEmailResponseModel>> ChangeEmail(UserSignInRequestModel request, string newEmail)
        {
            try
            {
                var response = await _authService.ChangeEmail(request, newEmail);

                return response;
            }

            catch (FirebaseException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("changePassword")]
        public async Task<ActionResult<ChangePasswordResponseModel>> ChangePassword(UserSignInRequestModel request, string newPassword)
        {
            try
            {
                var response = await _authService.ChangePassword(request, newPassword);

                return response;
            }

            catch (FirebaseException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
