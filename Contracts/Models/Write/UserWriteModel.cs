using System;
namespace Contracts.Models.Write
{
    public class UserWriteModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirebaseId { get; set; }

        public DateTime DateCreated { get; set; }

        public string UserName { get; set; }

        public string IdToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
