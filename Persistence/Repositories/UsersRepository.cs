using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.Models.Read;
using Contracts.Models.Write;

namespace Persistence.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private const string TableName = "Users";
        private readonly ISqlClient _sqlClient;

        public UsersRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }

        public Task<IEnumerable<UserReadModel>> GetAllUsers()
        {
            var sql = $"SELECT * FROM {TableName}";
            return _sqlClient.QueryAsync<UserReadModel>(sql);
        }

        public Task<UserReadModel> GetUserByFirebaseId(string firebaseId)
        {
            var sql = $"SELECT * FROM {TableName} WHERE FirebaseId = @firebaseId";
            return _sqlClient.QuerySingleOrDefaultAsync<UserReadModel>(sql, new
            {
                FirebaseId = firebaseId
            });
        }

        public Task<UserReadModel> GetUser(string userName, string password)
        {
            var sql = @$"SELECT * FROM {TableName} WHERE UserName = @UserName AND Password = @Password";

            return _sqlClient.QuerySingleOrDefaultAsync<UserReadModel>(sql, new
            {
                UserName = userName,
                Password = password
            });
        }

        public Task<UserReadModel> GetUserByName(string userName)
        {
            var sql = @$"SELECT * FROM {TableName} WHERE UserName = @UserName";

            return _sqlClient.QuerySingleOrDefaultAsync<UserReadModel>(sql, new
            {
                UserName = userName
            });
        }

        public Task<UserReadModel> GetUserByEmail(string email)
        {
            var sql = @$"SELECT * FROM {TableName} WHERE Email = @Email";

            return _sqlClient.QuerySingleOrDefaultAsync<UserReadModel>(sql, new
            {
                Email = email
            });
        }

        public Task<int> CreateUser(UserWriteModel user)
        {
            var sql = @$"INSERT INTO {TableName} (Id, FirebaseId, Email, DateCreated, UserName)
                        VALUES(@Id, @FirebaseId, @Email, @DateCreated, @UserName)
            ON DUPLICATE KEY UPDATE Email = @Email";

            return _sqlClient.ExecuteAsync(sql, user);

            //return _sqlClient.ExecuteAsync(sql, new UserReadModel
            //{
            //    Id = user.Id,
            //    FirebaseId = user.FirebaseId,
            //    Email = user.Email,
            //    DateCreated = user.DateCreated,
            //    UserName = user.UserName
            //});
        }

        public Task<int> DeleteUser(string email)
        {
            var sql = $"DELETE FROM {TableName} WHERE Email = @Email";

            return _sqlClient.ExecuteAsync(sql, new
            {
                Email = email
            });
        }
    }
}
