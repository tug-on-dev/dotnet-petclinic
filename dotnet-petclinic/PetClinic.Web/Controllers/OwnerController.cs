using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetClinic.Web.Data;
using PetClinic.Web.Models;
using PetClinic.Web.Models.ViewModels;

namespace PetClinic.Web.Controllers;

public class OwnerController : Controller
{
    private readonly PetClinicDbContext _context;
    private const int PageSize = 5;

    public OwnerController(PetClinicDbContext context)
    {
        _context = context;
    }

    // GET: Owner/Find
    public IActionResult Find()
    {
        return View(new OwnerSearchViewModel());
    }

    // GET: Owner
    public async Task<IActionResult> Index(string? lastName, int page = 1)
    {
        try
        {
            if (page < 1) page = 1;

            IQueryable<Owner> query = _context.Owners.Include(o => o.Pets);

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query.Where(o => o.LastName.Contains(lastName));
                ViewData["LastName"] = lastName;
            }

            query = query.OrderBy(o => o.LastName).ThenBy(o => o.FirstName);

            var paginatedList = PaginatedList<Owner>.Create(query, page, PageSize);

            if (paginatedList.Items.Count == 0 && !string.IsNullOrWhiteSpace(lastName))
            {
                TempData["ErrorMessage"] = $"No owners found with last name containing '{lastName}'";
                return RedirectToAction(nameof(Find));
            }

            if (paginatedList.Items.Count == 1 && !string.IsNullOrWhiteSpace(lastName))
            {
                return RedirectToAction(nameof(Details), new { id = paginatedList.Items[0].Id });
            }

            return View(paginatedList);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An error occurred while searching for owners.";
            return RedirectToAction(nameof(Find));
        }
    }

    // GET: Owner/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var owner = await _context.Owners
                .Include(o => o.Pets)
                    .ThenInclude(p => p.PetType)
                .Include(o => o.Pets)
                    .ThenInclude(p => p.Visits)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An error occurred while loading owner details.";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Owner/Create
    public IActionResult Create()
    {
        return View("CreateOrEdit", new Owner());
    }

    // POST: Owner/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Owner owner)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(owner);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Owner created successfully.";
                return RedirectToAction(nameof(Details), new { id = owner.Id });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while creating the owner.");
            }
        }
        return View("CreateOrEdit", owner);
    }

    // GET: Owner/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            return View("CreateOrEdit", owner);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An error occurred while loading the owner.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Owner/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Owner owner)
    {
        if (id != owner.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(owner);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Owner updated successfully.";
                return RedirectToAction(nameof(Details), new { id = owner.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OwnerExists(owner.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "The owner was modified by another user. Please try again.");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while updating the owner.");
            }
        }
        return View("CreateOrEdit", owner);
    }

    private bool OwnerExists(int id)
    {
        return _context.Owners.Any(e => e.Id == id);
    }
}
