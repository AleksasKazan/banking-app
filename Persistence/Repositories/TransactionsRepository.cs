using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.Enums;
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
            var sql = @$"INSERT INTO {TableName} (Id, UserId, Iban, CounterpartyId, CounterpartyIban, Amount, Balance, Type, Description, TimeStamp)
                        VALUES(@Id, @UserId, @Iban, @CounterpartyId, @CounterpartyIban, @Amount, @Balance, @Type, @Description, @TimeStamp)
                        ON DUPLICATE KEY UPDATE Balance=@Balance, Type=@Type, Description=@Description, TimeStamp=@TimeStamp";

            return _sqlClient.ExecuteAsync(sql, request);
        }

        public Task<IEnumerable<TransactionReadModel>> GetAllTransactions(Guid userId)
        {
            var sql = @$"SELECT * FROM {TableName} WHERE UserId=@UserId";

            return _sqlClient.QueryAsync<TransactionReadModel>(sql, new { UserId = userId });
        }

        public Task<IEnumerable<TransactionReadModel>> GetTransactionsByType(Guid userId, Transaction transaction)
        {
            var sql = @$"SELECT * FROM {TableName} WHERE UserId=@UserId and Type=@Transaction";

            return _sqlClient.QueryAsync<TransactionReadModel>(sql, new
            {
                UserId = userId,
                Transaction = transaction
            });
        }

        public Task<TransactionReadModel> GetLastTransaction(Guid userId)
        {
            var sql = @$"SELECT * FROM {TableName} WHERE UserId=@UserId ORDER BY TimeStamp DESC LIMIT 1";

            return _sqlClient.QuerySingleOrDefaultAsync<TransactionReadModel>(sql, new {UserId = userId});
        }

        public Task<int> DeletePending(Guid id)
        {
            var sql = $"DELETE FROM {TableName} WHERE Id=@Id";

            return _sqlClient.ExecuteAsync(sql, new {Id = id });
        }
    }
}
