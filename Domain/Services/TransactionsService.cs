using System;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.Models.Request;
using Contracts.Models.Response;
using Contracts.Models.Write;
using Persistence.Repositories;

namespace Domain.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IAccountsRepository _accountsRepository;
        private readonly ITransactionsRepository _transactionsRepository;

        public TransactionsService(IUsersRepository usersRepository,
            IAccountsRepository accountsRepository, ITransactionsRepository transactionsRepository)
        {
            _usersRepository = usersRepository;
            _accountsRepository = accountsRepository;
            _transactionsRepository = transactionsRepository;
        }

        public async Task<TopUpResponseModel> TopUp(TopUpRequestModel request, Guid userId)
        {
            var resp = await _accountsRepository.GetBalance(userId);
            var newBalance = resp.Balance + request.Amount;
            var updatedAccount = new AccountWriteModel
            {
                UserId = userId,
                Balance = newBalance,
                AccountNumber = resp.AccountNumber
            };
            await _accountsRepository.SaveOrUpdateAccount(updatedAccount);
            var newTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = request.Amount,
                CounterpartyId = userId,
                Type = Transaction.TopUp,
                Description = $"{Transaction.TopUp} from card: {request.CardNumber}",
                TimeStamp = DateTime.Now
            };
            await _transactionsRepository.SaveOrUpdate(newTransaction);

            return new TopUpResponseModel
            {
                Amount = request.Amount,
                Balance = newBalance
            };
        }

        public async Task<TransferResponseModel> Transfer(TransferRequestModel request, Guid userId)
        {
            var transferFees = 1;
            var resp = await _accountsRepository.GetBalance(userId);
            if (resp.Balance < request.Amount)
            {
                throw new Exception($"Your balance {resp.Balance} isn't enough for the transfer {request.Amount}. Top up balance please");
            }
            var newSenderBalance = resp.Balance - request.Amount - transferFees;
            var updatedAccount = new AccountWriteModel
            {
                UserId = userId,
                Balance = newSenderBalance,
                AccountNumber = resp.AccountNumber
            };
            await _accountsRepository.SaveOrUpdateAccount(updatedAccount);
            var newTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = request.Amount * (-1),
                CounterpartyId = Guid.Parse("1e1c7efb-8c34-4000-b4f8-01396f2caf60"),
                Type = Transaction.Transfer,
                Description = $"{resp.AccountNumber} {Transaction.Transfer} {request.Iban}",
                TimeStamp = DateTime.Now
            };
            var fees = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = -1,
                CounterpartyId = Guid.Parse("1e1c7efb-8c34-4000-b4f8-01396f2caf60"),
                Type = Transaction.Fees,
                Description = $"{resp.AccountNumber} {Transaction.Fees}",
                TimeStamp = DateTime.Now
            };
            await _transactionsRepository.SaveOrUpdate(fees);
            await _transactionsRepository.SaveOrUpdate(newTransaction);

            return new TransferResponseModel
            {
                Amount = request.Amount,
                Balance = newSenderBalance,
                Type = Transaction.Transfer,
                Description = $"From: {resp.AccountNumber} {Transaction.Transfer} to: {request.Iban}"
            };
        }

        public async Task<SendMoneyResponseModel> Send(SendMoneyRequestModel request, Guid userId)
        {
            var resp = await _usersRepository.GetUserByEmail(request.Email);
            if (resp.Email is null)
            {
                throw new NullReferenceException($"User email: {request.Email} not found");
            }
            var senderAccount = await _accountsRepository.GetBalance(userId);
            if (senderAccount.Balance < request.Amount)
            {
                throw new Exception($"Your balance {senderAccount.Balance} isn't enough for the transfer {request.Amount}. Top up balance please");
            }
            var newSenderBalance = senderAccount.Balance - request.Amount;
            var updatedSenderAccount = new AccountWriteModel
            {
                UserId = userId,
                Balance = newSenderBalance,
                AccountNumber = senderAccount.AccountNumber
            };
            await _accountsRepository.SaveOrUpdateAccount(updatedSenderAccount);

            var rceiverAccount = await _accountsRepository.GetBalance(resp.Id);
            var newReceiverBalance = rceiverAccount.Balance + request.Amount;
            var updatedReceiverAccount = new AccountWriteModel
            {
                UserId = userId,
                Balance = newReceiverBalance,
                AccountNumber = rceiverAccount.AccountNumber
            };
            await _accountsRepository.SaveOrUpdateAccount(updatedReceiverAccount);
           
            var newTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = request.Amount * (-1),
                CounterpartyId = resp.Id,
                Type = Transaction.SendMoney,
                Description = $"{rceiverAccount.AccountNumber} {Transaction.SendMoney} {senderAccount.AccountNumber}",
                TimeStamp = DateTime.Now
            };
            await _transactionsRepository.SaveOrUpdate(newTransaction);

            return new SendMoneyResponseModel
            {
                Amount = request.Amount,
                Balance = newSenderBalance,
                Type = Transaction.SendMoney,
                Description = $"{rceiverAccount.AccountNumber} {Transaction.SendMoney} {senderAccount.AccountNumber}"
            };
        }

        public async Task<RequestMoneyResponseModel> Request(ReceiveMoneyRequestModel request, Guid userId)
        {
            var resp = await _usersRepository.GetUserByEmail(request.Email);
            if (resp.Email is null)
            {
                throw new NullReferenceException($"User email: {resp.Email} not found");
            }

            var newTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = request.Amount,
                CounterpartyId = resp.Id,
                Type = Transaction.Pending,
                Description = $"{request.Email} {Transaction.Pending} {resp.UserName}",
                TimeStamp = DateTime.Now
            };
            await _transactionsRepository.SaveOrUpdate(newTransaction);

            return new RequestMoneyResponseModel
            {
                Amount = request.Amount,
                Type = Transaction.Pending,
                Description = $"{Transaction.Pending} from: {request.Email}"
            };
        }

        public async Task<SendMoneyResponseModel> ConfirmPending(ConfirmPendingRequestModel request, Guid userId)
        {
            var resp = await _usersRepository.GetUserByEmail(request.Email);
            var transactions = await _transactionsRepository.GetAllTransactions(userId);

            var pendingTransaction = transactions.Where(transaction => transaction.Type == Transaction.Pending
            && transaction.UserId == resp.Id).FirstOrDefault();
            if (pendingTransaction is null)
            {
                throw new NullReferenceException($"Money request from {resp.UserName} not found");
            }
            if (!request.Confirm)
            {
                var senderAccount = await _accountsRepository.GetBalance(userId);

                var newTransaction = new TransactionWriteModel
                {
                    Id = pendingTransaction.Id,
                    UserId = userId,
                    Amount = pendingTransaction.Amount * (-1),
                    CounterpartyId = resp.Id,
                    Type = Transaction.Cancelled,
                    Description = $"{resp.UserName} {Transaction.Cancelled}",
                    TimeStamp = DateTime.Now
                };
                await _transactionsRepository.SaveOrUpdate(newTransaction);

                return new SendMoneyResponseModel
                {
                    Amount = pendingTransaction.Amount * (-1),
                    Balance = senderAccount.Balance,
                    Type = Transaction.Cancelled,
                    Description = $"{resp.UserName} {Transaction.Cancelled}"
                };
            }
            else
            {
                var rceiverAccount = await _accountsRepository.GetBalance(resp.Id);
                var newReceiverBalance = rceiverAccount.Balance + pendingTransaction.Amount;
                var updatedReceiverAccount = new AccountWriteModel
                {
                    UserId = userId,
                    Balance = newReceiverBalance,
                    AccountNumber = rceiverAccount.AccountNumber
                };
                await _accountsRepository.SaveOrUpdateAccount(updatedReceiverAccount);

                var senderAccount = await _accountsRepository.GetBalance(userId);
                var newSenderBalance = senderAccount.Balance - pendingTransaction.Amount;
                var updatedSenderAccount = new AccountWriteModel
                {
                    UserId = userId,
                    Balance = newSenderBalance,
                    AccountNumber = senderAccount.AccountNumber
                };
                await _accountsRepository.SaveOrUpdateAccount(updatedSenderAccount);

                var newTransaction = new TransactionWriteModel
                {
                    Id = pendingTransaction.Id,
                    Type = Transaction.SendMoney,
                    Description = $"From: {senderAccount.AccountNumber} {Transaction.SendMoney} to: {rceiverAccount.AccountNumber}",
                    TimeStamp = DateTime.Now
                };
                await _transactionsRepository.SaveOrUpdate(newTransaction);

                return new SendMoneyResponseModel
                {
                    Amount = pendingTransaction.Amount * (-1),
                    Balance = newSenderBalance,
                    Type = Transaction.SendMoney,
                    Description = $"From: {senderAccount.AccountNumber} {Transaction.SendMoney} to: {rceiverAccount.AccountNumber}",
                };
            }
            
        }

    }
}
