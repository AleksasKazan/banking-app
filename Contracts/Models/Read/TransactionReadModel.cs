using System;
using Contracts.Enums;

namespace Contracts.Models.Read
{
    public class TransactionReadModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid CounterpartyId { get; set; }

        public decimal Amount { get; set; }

        public Transaction Type { get; set; }

        public string Description { get; set; }

        public DateTime TimeStamp { get; set; } 
    }
}
