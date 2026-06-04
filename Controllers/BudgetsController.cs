
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using ExpenseTracker.Data;

public class BudgetsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BudgetsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: BUDGETS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Budgets.ToListAsync());
    }

    // GET: BUDGETS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var budget = await _context.Budgets
            .FirstOrDefaultAsync(m => m.Id == id);
        if (budget == null)
        {
            return NotFound();
        }

        return View(budget);
    }

    // GET: BUDGETS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: BUDGETS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Budget budget)
    {
        // Current month
        string currentMonth = DateTime.Now.ToString("MMMM yyyy");

        // Assign month automatically
        budget.Month = currentMonth;

        // Check existing budget
        var existingBudget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Month == currentMonth);

        if (existingBudget != null)
        {
            // Update existing budget
            existingBudget.Amount = budget.Amount;

            _context.Update(existingBudget);
        }
        else
        {
            // Create new budget
            _context.Add(budget);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: BUDGETS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var budget = await _context.Budgets.FindAsync(id);
        if (budget == null)
        {
            return NotFound();
        }
        return View(budget);
    }

    // POST: BUDGETS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Amount")] Budget budget)
    {
        if (id != budget.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(budget);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BudgetExists(budget.Id))
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
        return View(budget);
    }

    // GET: BUDGETS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var budget = await _context.Budgets
            .FirstOrDefaultAsync(m => m.Id == id);
        if (budget == null)
        {
            return NotFound();
        }

        return View(budget);
    }

    // POST: BUDGETS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var budget = await _context.Budgets.FindAsync(id);
        if (budget != null)
        {
            _context.Budgets.Remove(budget);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BudgetExists(int? id)
    {
        return _context.Budgets.Any(e => e.Id == id);
    }
}
