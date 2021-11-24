using System.Threading.Tasks;
using Contracts.Models.Write;

namespace Persistence.Repositories
{
    public class CardsRepository : ICardsRepository
    {
        private const string TableName = "Cards";
        private readonly ISqlClient _sqlClient;

        public CardsRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public Task<int> CreateCard(CardWriteModel card)
        {
            var sql = @$"INSERT INTO {TableName} (CardNumber, UserId, ValidationDate, CVC, AccountNumber)
                        VALUES(@CardNumber, @UserId, @ValidationDate, @CVC), @AccountNumber";

            return _sqlClient.ExecuteAsync(sql, card);
        }
    }
}
