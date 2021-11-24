using System;
using System.Threading.Tasks;
using Contracts.Models.Request;
using Contracts.Models.Response;

namespace Domain.Services
{
    public interface ITransactionsService
    {
        Task<TopUpResponseModel> TopUp(TopUpRequestModel request, Guid userId);

        Task<TransferResponseModel> Transfer(TransferRequestModel request, Guid userId);

        Task<TransferResponseModel> Send(SendMoneyRequestModel request, Guid userId);

        Task<RequestMoneyResponseModel> Request(ReceiveMoneyRequestModel request, Guid userId);

        Task<TransferResponseModel> ConfirmPending(ConfirmPendingRequestModel request, Guid userId);
    }
}
