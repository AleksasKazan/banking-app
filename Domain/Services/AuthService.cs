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
        private readonly ITransactionsRepository _transactionsRepository;

        public AuthService(IFireBaseClient fireBaseClient,
            IUsersRepository usersRepository, IAccountsRepository accountsRepository, ITransactionsRepository transactionsRepository)
        {
            _fireBaseClient = fireBaseClient;
            _usersRepository = usersRepository;
            _accountsRepository = accountsRepository;
            _transactionsRepository = transactionsRepository;
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
                IsActive = true
            };

            var newAccount = new AccountWriteModel
            {
                Iban = accountNumber,
                UserId = userId,
                Balance = 0.00M,
                DateCreated = DateTime.Now
            };

            var newTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 0.00M,
                CounterpartyId = Guid.Empty,
                Type = Contracts.Enums.Transaction.Fees,
                Description = $"Account {accountNumber} opening",
                TimeStamp = DateTime.Now,
                Balance = 0.00M,
                Iban = accountNumber,
                CounterpartyIban = string.Empty
            };

            await Task.WhenAll(
                _usersRepository.CreateUser(newUser),
                _accountsRepository.SaveOrUpdateAccount(newAccount),
                _transactionsRepository.SaveOrUpdate(newTransaction));

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
            var response = await _usersRepository.GetUserByEmail(request.Email);
            var updatedUser = new UserWriteModel
            {
                Id = response.Id,
                FirebaseId = response.FirebaseId,
                Email = response.Email,
                UserName = response.UserName,
                IsActive = !response.IsActive
            };

            if (user.IdToken is not null)
            {
                await _usersRepository.DisableUser(updatedUser);
            }
            return new SuccessResponse
            {
                Message = $"User {user.Email} have been deleted / disabled"
            };
        }

        public async Task<PasswordResetResponseModel> PasswordReset(PasswordResetRequestModel request)
        {
            var response = await _fireBaseClient.PasswordReset(request);

            return new PasswordResetResponseModel
            {
                Email = response.Email
            };
        }

        public async Task<ChangeEmailResponseModel> ChangeEmail(UserSignInRequestModel request, string newEmail)
        {
            var response = await _fireBaseClient.SignIn(request);
            var response2 = await _fireBaseClient.ChangeEmail(newEmail, response.IdToken);
            var user = await _usersRepository.GetUserByEmail(request.Email);
            var updatedUser = new UserWriteModel
            {
                Id = user.Id,
                Email = newEmail
            };
            await _usersRepository.CreateUser(updatedUser);

            return new ChangeEmailResponseModel
            {
                Email = response2.Email
            };
        }

        public async Task<ChangePasswordResponseModel> ChangePassword(UserSignInRequestModel request, string newPassword)
        {
            var response = await _fireBaseClient.SignIn(request);
            var response2 = await _fireBaseClient.ChangePassword(newPassword, response.IdToken);

            return new ChangePasswordResponseModel
            {
                PasswordHash = response2.PasswordHash
            };
        }
    }
}
