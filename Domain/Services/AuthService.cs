using System;
using System.Threading.Tasks;
using Contracts.Models.Request;
using Contracts.Models.Response;
using Contracts.Models.Write;
using Domain.Clients.Firebase;
using Persistence.Repositories;

namespace Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IFireBaseClient _fireBaseClient;
        private readonly IUsersRepository _usersRepository;
        private readonly IAccountsRepository _accountsRepository;

        public AuthService(IFireBaseClient fireBaseClient,
            IUsersRepository usersRepository, IAccountsRepository accountsRepository)
        {
            _fireBaseClient = fireBaseClient;
            _usersRepository = usersRepository;
            _accountsRepository = accountsRepository;
        }

        public async Task<UserSignUpResponseModel> SignUpAsync(UserSignUpRequestModel request)
        {
            var user = await _fireBaseClient.SignUp(request);
            var accountNumber = "LT" + new Random().Next(0, 999999999).ToString()
                                    + new Random().Next(0, 999999999).ToString();
            var userId = Guid.NewGuid();
            var newUser = new UserWriteModel
            {
                Id = userId,
                FirebaseId = user.FirebaseId,
                Email = user.Email,
                DateCreated = DateTime.Now,
                UserName = request.UserName,
            };
            await _usersRepository.CreateUser(newUser);

            var newAccount = new AccountWriteModel
            {
                AccountNumber = accountNumber,
                UserId = userId,
                Balance = 0,
                DateCreated = DateTime.Now
            };
            await _accountsRepository.SaveOrUpdateAccount(newAccount);
            return new UserSignUpResponseModel
            {
                Id = newUser.Id,
                FirebaseId = newUser.FirebaseId,
                Email = newUser.Email,
                DateCreated = newUser.DateCreated,
                UserName = newUser.UserName,
                AccountNumber = accountNumber
            };
        }

        public async Task<UserSignInResponseModel> SignInAsync(UserSignInRequestModel request)
        {
            var user = await _fireBaseClient.SignIn(request);

            var userSql = await _usersRepository.GetUserByFirebaseId(user.FirebaseId);
            return new UserSignInResponseModel
            {
                UserName = userSql.UserName,
                FirebaseId = user.FirebaseId,
                IdToken = user.IdToken,
                Email = user.Email
            };
        }

        public async Task<SuccessResponse> DeleteAccount(UserSignInRequestModel request)
        {
            var user = await _fireBaseClient.SignIn(request);
            var resp = await _fireBaseClient.DeleteAccount(user.IdToken);
            if (user.IdToken is not null)
            {
                await _usersRepository.DeleteUser(request.Email);
            }
            return new SuccessResponse
            {
                Message = $"User {user.Email} have been deleted"
            };
        }

        public async Task<PasswordResetResponseModel> PasswordReset(PasswordResetRequestModel request)
        {
            var response = await _fireBaseClient.PasswordReset(request);

            //if (response.Email is null)
            //{
            //    throw new Exception($"The email address {request.Email} was not found");
            //}
            return new PasswordResetResponseModel
            {
                Email = response.Email
            };
        }

        public async Task<ChangeEmailResponseModel> ChangeEmail(UserSignInRequestModel request, string newEmail)
        {
            var response = await _fireBaseClient.SignIn(request);
            var response2 = await _fireBaseClient.ChangeEmail(newEmail, response.IdToken);

            //if (response.Email is null)
            //{
            //    throw new Exception($"The email address {request.Email} is already in use by another account");
            //}
            var updatedUser = new UserWriteModel
            {
                FirebaseId = response2.FirebaseId,
                Email = newEmail,
                UserName = newEmail
            };
            await _usersRepository.CreateUser(updatedUser);

            return new ChangeEmailResponseModel
            {
                Email = response.Email
            };
        }

        public async Task<ChangePasswordResponseModel> ChangePassword(UserSignInRequestModel request, string newPassword)
        {
            var response = await _fireBaseClient.SignIn(request);
            var response2 = await _fireBaseClient.ChangePassword(newPassword, response.IdToken);;

            //if (response.Email is null)
            //{
            //    throw new Exception($"The password {request.Password} is incorect");
            //}
            return new ChangePasswordResponseModel
            {
                Email = response.Email
            };
        }
    }
}
