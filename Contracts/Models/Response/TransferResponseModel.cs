using Contracts.Enums;

namespace Contracts.Models.Response
{
    public class TransferResponseModel
    {
        public decimal Balance { get; set; }

        public decimal Amount { get; set; }

        public decimal Fee { get; set; }

        public decimal NewBalance { get; set; }

        public Transaction Type { get; set; }

        public string Description { get; set; }

        public string Iban { get; set; }
    }
}
