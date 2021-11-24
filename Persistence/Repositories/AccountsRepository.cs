using System;
using System.Threading.Tasks;
using Contracts.Models.Read;
using Contracts.Models.Write;

namespace Persistence.Repositories
{
    public class AccountsRepository : IAccountsRepository
    {
        private const string TableName = "Accounts";
        private readonly ISqlClient _sqlClient;

        public AccountsRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public Task<int> SaveOrUpdateAccount(AccountWriteModel account)
        {
            var sql = @$"INSERT INTO {TableName} (Iban, UserId, Balance, DateCreated)
                        VALUES(@Iban, @UserId, @Balance, @DateCreated)
            ON DUPLICATE KEY UPDATE Balance = @Balance";

            return _sqlClient.ExecuteAsync(sql, account);
        }

        public Task<AccountReadModel> GetAccount(Guid userId)
        {
            var sql = $"SELECT * FROM {TableName} WHERE UserId = @UserId";

            return _sqlClient.QuerySingleOrDefaultAsync<AccountReadModel>(sql, new { UserId = userId });
        }

        public Task<AccountReadModel> GetUserByIban(string iban)
        {
            var sql = @$"SELECT * FROM {TableName} WHERE Iban = @Iban";

            return _sqlClient.QuerySingleOrDefaultAsync<AccountReadModel>(sql, new { Iban = iban });
        }
    }    
}
