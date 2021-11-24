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
        private readonly IFeesRepository _feesRepository;

        public TransactionsService(IUsersRepository usersRepository,
            IAccountsRepository accountsRepository, ITransactionsRepository transactionsRepository, IFeesRepository feesRepository)
        {
            _usersRepository = usersRepository;
            _accountsRepository = accountsRepository;
            _transactionsRepository = transactionsRepository;
            _feesRepository = feesRepository;
        }

        public async Task<TopUpResponseModel> TopUp(TopUpRequestModel request, Guid userId)
        {
            var account = await _accountsRepository.GetAccount(userId);

            var transaction = await _transactionsRepository.GetLastTransaction(userId);

            var newBalance = transaction.Balance + request.Amount;

            var updatedAccount = new AccountWriteModel
            {
                UserId = userId,
                Balance = newBalance,
                Iban = account.Iban
            };
            var newTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = request.Amount,
                CounterpartyId = Guid.Empty,
                Type = Transaction.TopUp,
                Description = $"{Transaction.TopUp} from card: {request.CardNumber}",
                TimeStamp = DateTime.Now,
                Balance = newBalance,
                Iban = account.Iban,
                CounterpartyIban = string.Empty
            };

            await Task.WhenAll(
                _accountsRepository.SaveOrUpdateAccount(updatedAccount),
                _transactionsRepository.SaveOrUpdate(newTransaction));

            return new TopUpResponseModel
            {
                Balance = transaction.Balance,
                Amount = request.Amount,
                NewBalance = newTransaction.Balance,
                Type = Transaction.TopUp,
                Description = newTransaction.Description
            };
        }

        public async Task<TransferResponseModel> Transfer(TransferRequestModel request, Guid userId)
        {
            var transferFees = await _feesRepository.GetFeeByTransactionType(Transaction.Transfer);

            var lastSenderTransaction = await _transactionsRepository.GetLastTransaction(userId);

            if (lastSenderTransaction.Balance < request.Amount + transferFees.Amount)
            {
                throw new Exception($"Your balance {lastSenderTransaction.Balance} isn't enough for the transfer {request.Amount}. Top up balance please");
            }

            var receiverAccount = await _accountsRepository.GetUserByIban(request.Iban);

            var newSenderBalance = lastSenderTransaction.Balance - request.Amount;

            var updatedSenderAccount = new AccountWriteModel
            {
                UserId = userId,
                Balance = newSenderBalance - transferFees.Amount,
                Iban = lastSenderTransaction.Iban
            };

            var newSenderTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = -request.Amount,
                CounterpartyId = receiverAccount is null ? Guid.Empty : receiverAccount.UserId,
                Type = Transaction.Transfer,
                Description = $"From: {lastSenderTransaction.Iban} {Transaction.Transfer} to: {request.Iban}",
                TimeStamp = DateTime.Now,
                Balance = newSenderBalance,
                Iban = lastSenderTransaction.Iban,
                CounterpartyIban = request.Iban
            };

            if (receiverAccount is not null)
            {
                var lastReceiverTransaction = await _transactionsRepository.GetLastTransaction(receiverAccount.UserId);

                var newReceiverBalance = lastReceiverTransaction.Balance + request.Amount;

                var updatedReceiverAccount = new AccountWriteModel
                {
                    UserId = receiverAccount.UserId,
                    Balance = newReceiverBalance,
                    Iban = receiverAccount.Iban
                };

                var updatedReceiverTransaction = new TransactionWriteModel
                {
                    Id = Guid.NewGuid(),
                    UserId = receiverAccount.UserId,
                    Amount = request.Amount,
                    CounterpartyId = userId,
                    Type = Transaction.Received,
                    Description = $"{receiverAccount.Iban} {Transaction.Received} from: {lastSenderTransaction.Iban}",
                    TimeStamp = DateTime.Now,
                    Balance = newReceiverBalance,
                    Iban = receiverAccount.Iban,
                    CounterpartyIban = lastSenderTransaction.Iban
                };

                await Task.WhenAll(
                _accountsRepository.SaveOrUpdateAccount(updatedReceiverAccount),
                _transactionsRepository.SaveOrUpdate(updatedReceiverTransaction));
            }
            
            var feesTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = -transferFees.Amount,
                CounterpartyId = Guid.Empty,
                Type = Transaction.Fees,
                Description = $"Transaction: {newSenderTransaction.Id} {Transaction.Fees}",
                TimeStamp = newSenderTransaction.TimeStamp.AddSeconds(1),
                Balance = newSenderBalance - transferFees.Amount,
                Iban = lastSenderTransaction.Iban,
                CounterpartyIban = string.Empty
            };

            await Task.WhenAll(
                _accountsRepository.SaveOrUpdateAccount(updatedSenderAccount),
                _transactionsRepository.SaveOrUpdate(newSenderTransaction),
                _transactionsRepository.SaveOrUpdate(feesTransaction));

            return new TransferResponseModel
            {
                Balance = lastSenderTransaction.Balance,
                Amount = request.Amount,
                Fee = transferFees.Amount,
                NewBalance = newSenderBalance - transferFees.Amount,
                Type = Transaction.Transfer,
                Description = $"From: {lastSenderTransaction.Iban} {Transaction.Transfer} to: {request.Iban}"
            };
        }

        public async Task<TransferResponseModel> Send(SendMoneyRequestModel request, Guid userId)
        {
            var transferFees = await _feesRepository.GetFeeByTransactionType(Transaction.Transfer);

            var receiver = await _usersRepository.GetUserByEmail(request.Email);

            if (receiver is null)
            {
                throw new NullReferenceException($"User email: {request.Email} not found");
            }

            var lastSenderTransaction = await _transactionsRepository.GetLastTransaction(userId);

            if (lastSenderTransaction.Balance < request.Amount + transferFees.Amount)
            {
                throw new Exception($"Your balance {lastSenderTransaction.Balance} isn't enough for the transfer {request.Amount}. Top up balance please");
            }

            var newSenderBalance = lastSenderTransaction.Balance - request.Amount;

            var updatedSenderAccount = new AccountWriteModel
            {
                UserId = userId,
                Balance = newSenderBalance - transferFees.Amount,
                Iban = lastSenderTransaction.Iban
            };

            var receiverAccount = await _accountsRepository.GetAccount(receiver.Id);
           
            var newSenderTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = -request.Amount,
                CounterpartyId = receiverAccount.UserId,
                Type = Transaction.SendMoney,
                Description = $"{lastSenderTransaction.Iban} {Transaction.SendMoney} to: {request.Email}",
                TimeStamp = DateTime.Now,
                Balance = newSenderBalance,
                Iban = lastSenderTransaction.Iban,
                CounterpartyIban = receiverAccount.Iban
            };

            var lastReceiverTransaction = await _transactionsRepository.GetLastTransaction(receiverAccount.UserId);

            var newReceiverBalance = lastReceiverTransaction.Balance + request.Amount;

            var updatedReceiverAccount = new AccountWriteModel
            {
                UserId = receiverAccount.UserId,
                Balance = newReceiverBalance,
                Iban = receiverAccount.Iban
            };

            var pendingReceiverTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = receiverAccount.UserId,
                Amount = request.Amount,
                CounterpartyId = userId,
                Type = Transaction.Received,
                Description = $"{receiverAccount.Iban} {Transaction.Received} from: {lastSenderTransaction.Iban}",
                TimeStamp = DateTime.Now,
                Balance = newReceiverBalance,
                Iban = receiverAccount.Iban,
                CounterpartyIban = lastSenderTransaction.Iban
            };

            await Task.WhenAll(
                _accountsRepository.SaveOrUpdateAccount(updatedReceiverAccount),
                _transactionsRepository.SaveOrUpdate(pendingReceiverTransaction));

            var feesTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = -transferFees.Amount,
                CounterpartyId = Guid.Empty,
                Type = Transaction.Fees,
                Description = $"Transaction: {newSenderTransaction.Id} {Transaction.Fees}",
                TimeStamp = newSenderTransaction.TimeStamp.AddSeconds(1),
                Balance = newSenderBalance - transferFees.Amount,
                Iban = lastSenderTransaction.Iban,
                CounterpartyIban = string.Empty
            };

            await Task.WhenAll(
                _accountsRepository.SaveOrUpdateAccount(updatedSenderAccount),
                _transactionsRepository.SaveOrUpdate(newSenderTransaction),
                _transactionsRepository.SaveOrUpdate(feesTransaction));

            return new TransferResponseModel
            {
                Balance = lastSenderTransaction.Balance,
                Amount = request.Amount,
                Fee = transferFees.Amount,
                NewBalance = newSenderBalance - transferFees.Amount,
                Type = Transaction.SendMoney,
                Description = $"From: {lastSenderTransaction.Iban} {Transaction.SendMoney} to: {request.Email}"
            };
        }

        public async Task<RequestMoneyResponseModel> Request(ReceiveMoneyRequestModel request, Guid userId)
        {
            var sender = await _usersRepository.GetUserByEmail(request.Email);

            if (sender is null)
            {
                throw new NullReferenceException($"User email: {request.Email} not found");
            }

            var receiver = await _usersRepository.GetUserById(userId);

            var pendingTransactions = await _transactionsRepository.GetTransactionsByType(userId, Transaction.Pending);

            var isAlreadyRequested = pendingTransactions.Where(transaction => transaction.CounterpartyId == sender.Id).SingleOrDefault();

            if (isAlreadyRequested is not null)
            {
                throw new Exception($"You already requested {isAlreadyRequested.Amount} from {sender.Email}");
            }

            var lastSenderTransaction = await _transactionsRepository.GetLastTransaction(sender.Id);

            var lastReceiverTransaction = await _transactionsRepository.GetLastTransaction(userId);

            var pendingSenderTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = sender.Id,
                Amount = request.Amount,
                CounterpartyId = userId,
                Type = Transaction.Pending,
                Description = $"{Transaction.Pending} request from: {receiver.Email}",
                TimeStamp = DateTime.Now,
                Balance = lastSenderTransaction.Balance,
                Iban = lastSenderTransaction.Iban,
                CounterpartyIban = lastReceiverTransaction.Iban
            };

            var pendingReceiverTransaction = new TransactionWriteModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = request.Amount,
                CounterpartyId = sender.Id,
                Type = Transaction.Pending,
                Description = $"{Transaction.Pending} from: {request.Email}",
                TimeStamp = DateTime.Now,
                Balance = lastReceiverTransaction.Balance,
                Iban = lastReceiverTransaction.Iban,
                CounterpartyIban = lastSenderTransaction.Iban
            };

            await Task.WhenAll(
                _transactionsRepository.SaveOrUpdate(pendingSenderTransaction),
                _transactionsRepository.SaveOrUpdate(pendingReceiverTransaction));

            return new RequestMoneyResponseModel
            {
                Amount = request.Amount,
                Type = Transaction.Pending,
                Description = $"{Transaction.Pending} from: {request.Email}"
            };
        }

        public async Task<TransferResponseModel> ConfirmPending(ConfirmPendingRequestModel request, Guid userId)
        {
            var transferFees = await _feesRepository.GetFeeByTransactionType(Transaction.SendMoney);

            var lastSenderTransaction = await _transactionsRepository.GetLastTransaction(userId);

            var receiver = await _usersRepository.GetUserByEmail(request.Email);

            if (receiver is null)
            {
                throw new NullReferenceException($"User email: {request.Email} not found");
            }

            var pendingSenderTransactions = await _transactionsRepository.GetTransactionsByType(userId, Transaction.Pending);
            var pendingReceiverTransactions = await _transactionsRepository.GetTransactionsByType(receiver.Id, Transaction.Pending);

            var pendingSenderTransaction = pendingSenderTransactions.Where(transaction => transaction.CounterpartyId == receiver.Id).SingleOrDefault();
            var pendingReceiverTransaction = pendingReceiverTransactions.Where(transaction => transaction.CounterpartyId == userId).SingleOrDefault();

            if (pendingSenderTransaction is null)
            {
                throw new NullReferenceException($"Money request from {receiver.Email} not found");
            }

            if (!request.Confirm)
            {
                await Task.WhenAll(
                    _transactionsRepository.DeletePending(pendingSenderTransaction.Id),
                    _transactionsRepository.DeletePending(pendingReceiverTransaction.Id));

                return new TransferResponseModel
                {
                    Balance = lastSenderTransaction.Balance,
                    Amount = pendingSenderTransaction.Amount,
                    NewBalance = lastSenderTransaction.Balance,
                    Type = Transaction.Cancelled,
                    Description = $"{receiver.Email} request was {Transaction.Cancelled}"
                };                
            }

            else
            {
                if (lastSenderTransaction.Balance < pendingReceiverTransaction.Amount + transferFees.Amount)
                {
                    throw new Exception($"Your balance {lastSenderTransaction.Balance} isn't enough for the transfer {pendingReceiverTransaction.Amount}. Top up balance please");
                }

                var receiverAccount = await _accountsRepository.GetAccount(receiver.Id);
                var newReceiverBalance = receiverAccount.Balance + pendingSenderTransaction.Amount;
                var updatedReceiverAccount = new AccountWriteModel
                {
                    UserId = userId,
                    Balance = newReceiverBalance,
                    Iban = receiverAccount.Iban
                };

                var senderAccount = await _accountsRepository.GetAccount(userId);
                var newSenderBalance = senderAccount.Balance + pendingSenderTransaction.Amount;
                var updatedSenderAccount = new AccountWriteModel
                {
                    UserId = userId,
                    Balance = newSenderBalance,
                    Iban = senderAccount.Iban
                };

                var newSenderTransaction = new TransactionWriteModel
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Amount = pendingReceiverTransaction.Amount,
                    CounterpartyId = receiverAccount.UserId,
                    Type = Transaction.SendMoney,
                    Description = $"From: {lastSenderTransaction.Iban} {Transaction.SendMoney} to: {receiverAccount.Iban}",
                    TimeStamp = DateTime.Now,
                    Balance = newSenderBalance,
                    Iban = lastSenderTransaction.Iban,
                    CounterpartyIban = receiverAccount.Iban
                };

                var newReceiverTransaction = new TransactionWriteModel
                {
                    Id = Guid.NewGuid(),
                    UserId = receiverAccount.UserId,
                    Amount = pendingReceiverTransaction.Amount,
                    CounterpartyId = userId,
                    Type = Transaction.Received,
                    Description = $"{receiverAccount.Iban} {Transaction.Received} from: {lastSenderTransaction.Iban}",
                    TimeStamp = DateTime.Now,
                    Balance = newReceiverBalance,
                    Iban = receiverAccount.Iban,
                    CounterpartyIban = lastSenderTransaction.Iban
                };

                var feesTransaction = new TransactionWriteModel
                {
                    Id = Guid.NewGuid(),
                    UserId = receiverAccount.UserId,
                    Amount = transferFees.Amount,
                    CounterpartyId = Guid.Empty,
                    Type = Transaction.Fees,
                    Description = $"Transaction: {newReceiverTransaction.Id} {Transaction.Fees}",
                    TimeStamp = newReceiverTransaction.TimeStamp.AddSeconds(1),
                    Balance = newReceiverBalance - transferFees.Amount,
                    Iban = newReceiverTransaction.Iban,
                    CounterpartyIban = string.Empty
                };

                await Task.WhenAll(
                    _accountsRepository.SaveOrUpdateAccount(updatedReceiverAccount),
                    _accountsRepository.SaveOrUpdateAccount(updatedSenderAccount),
                    _transactionsRepository.SaveOrUpdate(newSenderTransaction),
                    _transactionsRepository.SaveOrUpdate(newReceiverTransaction),
                    _transactionsRepository.SaveOrUpdate(feesTransaction));

                return new TransferResponseModel
                {
                    Balance = lastSenderTransaction.Balance,
                    Amount = pendingSenderTransaction.Amount,
                    NewBalance = newSenderBalance,
                    Type = Transaction.SendMoney,
                    Description = $"From: {lastSenderTransaction.Iban} {Transaction.SendMoney} to: {receiverAccount.Iban}",
                };
            }
        }
    }
}
