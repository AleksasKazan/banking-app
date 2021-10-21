﻿using System;
namespace Contracts.Models.Write
{
    public class AccountWriteModel
    {
        public string AccountNumber { get; set; }

        public Guid UserId { get; set; }

        public decimal Balance { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
