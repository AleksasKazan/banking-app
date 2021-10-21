using System;
using Contracts.Enums;

namespace Contracts.Models.Response
{
    public class SendMoneyResponseModel
    {
        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public Transaction Type { get; set; }

        public string Description { get; set; }
    }
}
