using Contracts.Enums;

namespace Contracts.Models.Response
{
    public class RequestMoneyResponseModel
    {
        public decimal Amount { get; set; }

        public Transaction Type { get; set; }

        public string Description { get; set; }
    }
}
