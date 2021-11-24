using System;
using Contracts.Enums;

namespace Contracts.Models.Read
{
    public class FeeReadModel
    {
        public Guid Id { get; init; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public Transaction TransactionType { get; set; }
    }
}
