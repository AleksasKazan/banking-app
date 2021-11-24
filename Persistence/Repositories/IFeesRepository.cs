using System.Threading.Tasks;
using Contracts.Enums;
using Contracts.Models.Read;

namespace Persistence.Repositories
{
    public interface IFeesRepository
    {
        Task<FeeReadModel> GetFeeByTransactionType(Transaction transaction);
    }
}
