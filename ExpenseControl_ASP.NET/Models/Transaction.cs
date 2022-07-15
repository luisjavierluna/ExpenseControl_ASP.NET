﻿using System.ComponentModel.DataAnnotations;

namespace ExpenseControl_ASP.NET.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Category is a required field")]
        public int CategoryId { get; set; }
        [StringLength(maximumLength: 1000, ErrorMessage = "The note cannot be longer than {1} characters")]
        public string Note { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Account is a required field")]
        public int AccountId { get; set; }

    }
}
