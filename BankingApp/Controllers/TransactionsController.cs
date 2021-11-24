using System.Linq;
using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.Models.Request;
using Contracts.Models.Response;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _transactionsService;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IUsersRepository _usersRepository;

        public TransactionsController(ITransactionsService transactionsService,
            IUsersRepository usersRepository, ITransactionsRepository transactionsRepository)
        {
            _transactionsService = transactionsService;
            _transactionsRepository = transactionsRepository;
            _usersRepository = usersRepository;
        }

        [Authorize]
        [HttpPost]
        [Route("topUp")]
        public async Task<ActionResult<TopUpResponseModel>> TopUp(TopUpRequestModel request)
        {
            var firebaseId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id").Value;

            var user = await _usersRepository.GetUserByFirebaseId(firebaseId);

            var response = await _transactionsService.TopUp(request, user.Id);

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("transfer")]
        public async Task<ActionResult<TransferResponseModel>> Transfer(TransferRequestModel request)
        {
            var firebaseId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id").Value;

            var user = await _usersRepository.GetUserByFirebaseId(firebaseId);

            var response = await _transactionsService.Transfer(request, user.Id);

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("sendMoney")]
        public async Task<ActionResult<TransferResponseModel>> Send(SendMoneyRequestModel request)
        {
            var firebaseId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id").Value;

            var user = await _usersRepository.GetUserByFirebaseId(firebaseId);

            var response = await _transactionsService.Send(request, user.Id);

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("requestMoney")]
        public async Task<ActionResult<RequestMoneyResponseModel>> RequestMoney(ReceiveMoneyRequestModel request)
        {
            var firebaseId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id").Value;

            var user = await _usersRepository.GetUserByFirebaseId(firebaseId);

            var response = await _transactionsService.Request(request, user.Id);

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("confirmPending")]
        public async Task<ActionResult<TransferResponseModel>> ConfirmPending(ConfirmPendingRequestModel request)
        {
            var firebaseId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id").Value;

            var user = await _usersRepository.GetUserByFirebaseId(firebaseId);

            var response = await _transactionsService.ConfirmPending(request, user.Id);

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("statement")]
        public async Task<ActionResult<TransactionResponseModel>> GetTransactions()
        {
            var firebaseId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id").Value;

            var user = await _usersRepository.GetUserByFirebaseId(firebaseId);

            var transactions = await _transactionsRepository.GetAllTransactions(user.Id);

            return Ok(transactions);
        }

        [Authorize]
        [HttpGet]
        [Route("pendingRequests")]
        public async Task<ActionResult<TransactionResponseModel>> GetPendingTransactions()
        {
            var firebaseId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id").Value;

            var user = await _usersRepository.GetUserByFirebaseId(firebaseId);

            var pendingTransaction = await _transactionsRepository.GetTransactionsByType(user.Id, Transaction.Pending);

            return Ok(pendingTransaction);
        }

        [Authorize]
        [HttpGet]
        [Route("statement/sort")]
        public async Task<ActionResult<TransactionResponseModel>> GetTransactionsByDate([FromQuery]TransactionRequestModel requestModel)
        {
            var firebaseId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id").Value;

            var user = await _usersRepository.GetUserByFirebaseId(firebaseId);

            var transactions = await _transactionsRepository.GetAllTransactions(user.Id);

            var response = transactions
                .Where(transaction => transaction.TimeStamp >= requestModel.TimeStampFrom && transaction.TimeStamp <= requestModel.TimeStampTo
                                    && transaction.Amount >= requestModel.AmountFrom && transaction.Amount <= requestModel.AmountTo);

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("balance")]
        public async Task<ActionResult<string>> GetBalance()
        {
            var firebaseId = HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "user_id").Value;

            var user = await _usersRepository.GetUserByFirebaseId(firebaseId);

            var response = await _transactionsRepository.GetLastTransaction(user.Id);

            return Ok(response is not null ? $"Current balance is: {response.Balance}" : $"Current balance is: {decimal.Zero}");
        }
    }
}
