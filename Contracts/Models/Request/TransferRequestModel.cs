﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Models.Request
{
    public class TransferRequestModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(20)]
        public string Iban { get; set; }
    }
}
