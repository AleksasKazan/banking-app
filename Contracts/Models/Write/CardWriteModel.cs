using System;
namespace Contracts.Models.Write
{
    public class CardWriteModel
    {
        public string CardNumber { get; set; }

        public Guid UserId { get; set; }

        public DateTime ValidationDate { get; set; }

        public string Cvc { get; set; }

        public string AccountNumber { get; set; }
    }
}
