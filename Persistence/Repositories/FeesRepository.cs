using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.Models.Read;

namespace Persistence.Repositories
{
    public class FeesRepository : IFeesRepository
    {
        private const string TableName = "Fees";
        private readonly ISqlClient _sqlClient;

        public FeesRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public Task<FeeReadModel> GetFeeByTransactionType(Transaction transaction)
        {
            var sql = @$"SELECT * FROM {TableName} WHERE TransactionType=@transaction";

            return _sqlClient.QuerySingleOrDefaultAsync<FeeReadModel>(sql, new { Transaction = transaction });
        }
    }
}
