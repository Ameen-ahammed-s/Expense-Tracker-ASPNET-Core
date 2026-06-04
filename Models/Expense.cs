using System;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public decimal Amount { get; set; }

        public string? Category { get; set; }

        public DateTime Date { get; set; }
        public string? Month { get; set; }
    }
}