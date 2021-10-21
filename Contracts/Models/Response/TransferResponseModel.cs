using System;
using Contracts.Enums;

namespace Contracts.Models.Response
{
    public class TransferResponseModel
    {
        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public Transaction Type { get; set; }

        public string Description { get; set; }

        public string Iban { get; set; }
    }
}
