using System;
using Contracts.Enums;

namespace Contracts.Models.Response
{
    public class TransactionResponseModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Iban { get; set; }

        public string CounterpartyIban { get; set; }

        public Guid CounterpartyId { get; set; }

        public decimal Amount { get; set; }

        public Transaction Type { get; set; }

        public string Description { get; set; }

        public DateTime TimeStamp { get; set; }

        public decimal Balance { get; set; }
    }
}
