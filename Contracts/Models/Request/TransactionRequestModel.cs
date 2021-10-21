using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Models.Request
{
    public class TransactionRequestModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal AmountFrom { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal AmountTo { get; set; }

        [Timestamp]
        public DateTime TimeStampFrom { get; set; }

        [Timestamp]
        public DateTime TimeStampTo { get; set; }
    }
}
