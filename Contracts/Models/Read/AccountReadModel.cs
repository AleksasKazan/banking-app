using System;
namespace Contracts.Models.Read
{
    public class AccountReadModel
    {
        public string AccountNumber { get; set; }

        public Guid UserId { get; set; }

        public decimal Balance { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
