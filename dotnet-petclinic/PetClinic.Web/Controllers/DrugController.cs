using Microsoft.AspNetCore.Mvc;
using PetClinic.Web.Data;
using PetClinic.Web.Models;
using PetClinic.Web.Models.ViewModels;

namespace PetClinic.Web.Controllers;

public class DrugController : Controller
{
    private readonly PetClinicDbContext _dbContext;
    private const int ItemsPerPage = 10;

    public DrugController(PetClinicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult Index(int page = 1)
    {
        if (page < 1) page = 1;

        var drugsQuery = _dbContext.Drugs.OrderBy(drug => drug.Name);
        var drugsList = PaginatedList<Drug>.Create(drugsQuery, page, ItemsPerPage);

        return View(drugsList);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Drug newDrug)
    {
        if (!ModelState.IsValid)
        {
            return View(newDrug);
        }

        _dbContext.Drugs.Add(newDrug);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var existingDrug = await _dbContext.Drugs.FindAsync(id);
        if (existingDrug == null) return NotFound();
        
        return View(existingDrug);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Drug updatedDrug)
    {
        if (id != updatedDrug.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            return View(updatedDrug);
        }

        _dbContext.Drugs.Update(updatedDrug);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var drugToDelete = await _dbContext.Drugs.FindAsync(id);
        if (drugToDelete == null) return NotFound();

        return View(drugToDelete);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var drugToRemove = await _dbContext.Drugs.FindAsync(id);
        if (drugToRemove != null)
        {
            _dbContext.Drugs.Remove(drugToRemove);
            await _dbContext.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}
