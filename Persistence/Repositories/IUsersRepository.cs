using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.Models.Read;
using Contracts.Models.Write;

namespace Persistence.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<UserReadModel>> GetAllUsers();

        Task<UserReadModel> GetUserByFirebaseId(string firebaseId);

        Task<UserReadModel> GetUserById(Guid id);

        Task<int> CreateUser(UserWriteModel user);

        Task<int> DeleteUser(string email);

        Task<int> DisableUser(UserWriteModel user);

        Task<UserReadModel> GetUser(string userName, string password);

        Task<UserReadModel> GetUserByName(string userName);

        Task<UserReadModel> GetUserByEmail(string email);
    }
}
