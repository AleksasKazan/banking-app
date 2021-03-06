using System;
namespace Contracts.Models.Read
{
    public class UserReadModel
    {
        public Guid Id { get; init; }

        public string Email { get; set; }

        public string FirebaseId { get; set; }

        public DateTime DateCreated { get; set; }

        public string UserName { get; set; }

        public bool IsActive { get; set; }
    }
}
