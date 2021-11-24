using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Models.Request
{
    public class TransactionRequestModel
    {
        public decimal AmountFrom { get; set; } = decimal.MinValue;

        public decimal AmountTo { get; set; } = decimal.MaxValue;

        [Timestamp]
        public DateTime TimeStampFrom { get; set; } = DateTime.MinValue;

        [Timestamp]
        public DateTime TimeStampTo { get; set; } = DateTime.Now;
    }
}
