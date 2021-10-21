using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.Models.Read;
using Contracts.Models.Write;

namespace Persistence.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private const string TableName = "Transactions";
        private readonly ISqlClient _sqlClient;

        public TransactionsRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public Task<int> SaveOrUpdate(TransactionWriteModel request)
        {
            var sql = @$"INSERT INTO {TableName} (Id, UserId, CounterpartyId, Amount, Type, Description, TimeStamp)
                        VALUES(@Id, @UserId, @CounterpartyId, @Amount, @Type, @Description, @TimeStamp)
                        ON DUPLICATE KEY UPDATE Type=Type, Description=@Description, TimeStamp=@TimeStamp";

            return _sqlClient.ExecuteAsync(sql, request);
        }

        public Task<IEnumerable<TransactionReadModel>> GetAllTransactions(Guid userId)
        {
            var sql = @$"SELECT * FROM {TableName} WHERE CounterpartyId=@UserId or UserId=@UserId";
            return _sqlClient.QueryAsync<TransactionReadModel>(sql, new
            {
                CounterpartyId = userId,
                UserId = userId
            });
        }
    }
}
