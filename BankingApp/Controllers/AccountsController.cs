using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult<string>> GetBalance()
        {
            var firebaseId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id").Value;

            var user = await _usersRepository.GetUserByFirebaseId(firebaseId);

            var account = await _accountsRepository.GetAccount(user.Id);

            return Ok($"Your current balance is: {account.Balance}");
        }
    }
}
