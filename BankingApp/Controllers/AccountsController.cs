using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountsController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IAccountsRepository _accountsRepository;

        public AccountsController(IUsersRepository usersRepository, IAccountsRepository accountsRepository)
        {
            _usersRepository = usersRepository;
            _accountsRepository = accountsRepository;
        }

        [Authorize]
        [HttpGet]
        [Route("balance")]
        public async Task<ActionResult<AccountResponseModel>> GetBalance()
        {
            var firebaseId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id").Value;
            var user = await _usersRepository.GetUserByFirebaseId(firebaseId);

            var response = await _accountsRepository.GetBalance(user.Id);
            return Ok(new AccountResponseModel
            {
                AccountNumber = response.AccountNumber,
                Balance = response.Balance
            });
        }
    }
}
