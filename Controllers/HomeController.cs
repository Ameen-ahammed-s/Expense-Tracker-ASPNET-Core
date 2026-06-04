using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string currentMonth = DateTime.Now.ToString("MMMM yyyy");

            // Monthly expenses
            var expenses = await _context.Expenses
                .Where(e => e.Month == currentMonth)
                .ToListAsync();

            // Total expense
            decimal totalExpense = expenses.Sum(e => e.Amount);

            // Monthly budget
            var budgetData = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Month == currentMonth);

            decimal budget = budgetData != null
                ? budgetData.Amount
                : 0;

            decimal remaining = budget - totalExpense;

            // Category analytics
            var categoryData = expenses
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(x => x.Amount)
                })
                .ToList();

            ViewBag.CategoryLabels =
                JsonSerializer.Serialize(categoryData.Select(x => x.Category));

            ViewBag.CategoryAmounts =
                JsonSerializer.Serialize(categoryData.Select(x => x.Total));

            // Dashboard data
            ViewBag.CurrentMonth = currentMonth;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.Budget = budget;
            ViewBag.Remaining = remaining;

            return View();
        }
    }
}