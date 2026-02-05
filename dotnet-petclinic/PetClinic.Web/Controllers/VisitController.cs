using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetClinic.Web.Data;
using PetClinic.Web.Models;

namespace PetClinic.Web.Controllers;

public class VisitController : Controller
{
    private readonly PetClinicDbContext _context;

    public VisitController(PetClinicDbContext context)
    {
        _context = context;
    }

    // GET: /owners/{ownerId}/pets/{petId}/visits/new
    [HttpGet("/owners/{ownerId}/pets/{petId}/visits/new")]
    public async Task<IActionResult> Create(int ownerId, int petId)
    {
        var pet = await _context.Pets
            .Include(p => p.Owner)
            .Include(p => p.PetType)
            .Include(p => p.Visits.OrderByDescending(v => v.VisitDate))
            .FirstOrDefaultAsync(p => p.Id == petId && p.OwnerId == ownerId);

        if (pet == null)
        {
            return NotFound();
        }

        var visit = new Visit
        {
            PetId = petId,
            VisitDate = DateTime.Today,
            Pet = pet
        };

        return View(visit);
    }

    // POST: /owners/{ownerId}/pets/{petId}/visits/new
    [HttpPost("/owners/{ownerId}/pets/{petId}/visits/new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int ownerId, int petId, DateTime date, string description)
    {
        // Map the lowercase field names to Visit properties
        var visit = new Visit
        {
            PetId = petId,
            VisitDate = date,
            Description = description
        };

        if (ModelState.IsValid)
        {
            _context.Visits.Add(visit);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Visit added successfully!";
            return RedirectToAction("Details", "Owner", new { id = ownerId });
        }

        var pet = await _context.Pets
            .Include(p => p.Owner)
            .Include(p => p.PetType)
            .Include(p => p.Visits.OrderByDescending(v => v.VisitDate))
            .FirstOrDefaultAsync(p => p.Id == petId && p.OwnerId == ownerId);

        if (pet == null)
        {
            return NotFound();
        }

        visit.Pet = pet;
        return View(visit);
    }
}
