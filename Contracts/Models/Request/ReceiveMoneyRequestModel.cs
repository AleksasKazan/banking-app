using System.ComponentModel.DataAnnotations;

namespace Contracts.Models.Request
{
    public class ReceiveMoneyRequestModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal Amount { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
