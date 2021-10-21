using System;
namespace Contracts.Models.Read
{
    public class CardReadModel
    {
        public string CardNumber { get; set; }

        public Guid UserId { get; set; }

        public DateTime ValidationDate { get; set; }

        public string CVC { get; set; }

        public string AccountNumber { get; set; }
    }
}
