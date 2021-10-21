using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Models.Request
{
    public class TopUpRequestModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal Amount { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [Timestamp]
        public DateTime ValidationDate { get; set; }

        [Required]
        [StringLength(3)]
        public string CVC { get; set; }
    }
}
