using System.ComponentModel.DataAnnotations;

namespace Contracts.Models.Request
{
    public class ConfirmPendingRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool Confirm { get; set; }
    }
}
