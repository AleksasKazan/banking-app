using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.Models.Read;
using Contracts.Models.Request;
using Contracts.Models.Response;
using Contracts.Models.Write;

namespace Persistence.Repositories
{
    public interface ITransactionsRepository
    {
        Task<int> SaveOrUpdate(TransactionWriteModel request);

        Task<IEnumerable<TransactionReadModel>> GetAllTransactions(Guid userId);

        //Task<TransactionReadModel> GetTransactionByFirebaseId(string firebaseId);

        //Task<TransactionReadModel> GetTransaction(string userName, string password);

        //Task<TransactionReadModel> GetTransactionByName(string userName);
    }
}
