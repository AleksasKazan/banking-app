using System;
using System.Threading.Tasks;
using Contracts.Models.Read;
using Contracts.Models.Write;

namespace Persistence.Repositories
{
    public interface IAccountsRepository
    {
        Task<int> SaveOrUpdateAccount(AccountWriteModel account);

        Task<AccountReadModel> GetAccount(Guid userId);

        Task<AccountReadModel> GetUserByIban(string iban);
    }
}
