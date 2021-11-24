using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.Models.Read;
using Contracts.Models.Write;

namespace Persistence.Repositories
{
    public interface ITransactionsRepository
    {
        Task<int> SaveOrUpdate(TransactionWriteModel request);

        Task<IEnumerable<TransactionReadModel>> GetAllTransactions(Guid userId);

        Task<IEnumerable<TransactionReadModel>> GetTransactionsByType(Guid userId, Transaction transaction);

        Task<TransactionReadModel> GetLastTransaction(Guid userId);

        Task<int> DeletePending(Guid id);
    }
}
