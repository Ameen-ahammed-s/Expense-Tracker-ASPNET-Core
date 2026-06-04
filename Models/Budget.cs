using System;

namespace ExpenseTracker.Models
{
    public class Budget
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }
        public string? Month { get; set; }
    }
}