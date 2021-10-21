using System;
using System.ComponentModel.DataAnnotations;
using Contracts.Enums;

namespace Contracts.Models.Response
{
    public class TransactionResponseModel
    {
        public Guid Id { get; set; }

        public Guid CounterpartyId { get; set; }

        public decimal Amount { get; set; }

        public Transaction Type { get; set; }

        public string Description { get; set; }

        public DateTime TimeStamp { get; set; }

        public string UserName { get; set; }
    }
}
