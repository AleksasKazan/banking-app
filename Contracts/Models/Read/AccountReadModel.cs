using System;
namespace Contracts.Models.Read
{
    public class AccountReadModel
    {
        public string Iban { get; init; }

        public Guid UserId { get; init; }

        public decimal Balance { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
