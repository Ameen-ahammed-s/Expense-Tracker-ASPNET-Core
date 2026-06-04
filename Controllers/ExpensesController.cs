using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using ExpenseTracker.Data;
[Authorize]
public class ExpensesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ExpensesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: EXPENSES
    public async Task<IActionResult> Index()    
    {
        string currentMonth = DateTime.Now.ToString("MMMM yyyy");

        // Current month expenses
        var expenses = await _context.Expenses
            .Where(e => e.Month == currentMonth)
            .ToListAsync();

        // Total expense
        decimal totalExpense = expenses.Sum(e => e.Amount);

        // Current month budget
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
            System.Text.Json.JsonSerializer.Serialize(
                categoryData.Select(x => x.Category));

        ViewBag.CategoryAmounts =
            System.Text.Json.JsonSerializer.Serialize(
                categoryData.Select(x => x.Total));

        ViewBag.TotalExpense = totalExpense;
        ViewBag.Budget = budget;
        ViewBag.Remaining = remaining;
        ViewBag.CurrentMonth = currentMonth;

        return View(expenses);
    }

    // GET: EXPENSES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var expense = await _context.Expenses
            .FirstOrDefaultAsync(m => m.Id == id);
        if (expense == null)
        {
            return NotFound();
        }

        return View(expense);
    }

    // GET: EXPENSES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: EXPENSES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Expense expense)
    {
        if (ModelState.IsValid)
        {
            expense.Month = expense.Date.ToString("MMMM yyyy");

            _context.Add(expense);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(expense);
    }

    // POST: EXPENSES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Amount,Category,Date")] Expense expense)
    {
        if (id != expense.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(expense);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(expense.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(expense);
    }

    // GET: EXPENSES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var expense = await _context.Expenses
            .FirstOrDefaultAsync(m => m.Id == id);
        if (expense == null)
        {
            return NotFound();
        }

        return View(expense);
    }

    // POST: EXPENSES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense != null)
        {
            _context.Expenses.Remove(expense);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ExpenseExists(int? id)
    {
        return _context.Expenses.Any(e => e.Id == id);
    }
}
