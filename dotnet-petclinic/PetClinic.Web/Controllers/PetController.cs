using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetClinic.Web.Data;
using PetClinic.Web.Models;

namespace PetClinic.Web.Controllers;

public class PetController : Controller
{
    private readonly PetClinicDbContext _context;

    public PetController(PetClinicDbContext context)
    {
        _context = context;
    }

    // GET: /owners/{ownerId}/pets/new
    [HttpGet("/owners/{ownerId}/pets/new")]
    public async Task<IActionResult> Create(int ownerId)
    {
        var owner = await _context.Owners.FindAsync(ownerId);
        if (owner == null)
        {
            TempData["ErrorMessage"] = "Owner not found.";
            return RedirectToAction("Index", "Owner");
        }

        ViewBag.Owner = owner;
        ViewBag.PetTypes = new SelectList(await _context.PetTypes.OrderBy(pt => pt.Name).ToListAsync(), "Id", "Name");
        
        return View("CreateOrEdit", new Pet { OwnerId = ownerId });
    }

    // POST: /owners/{ownerId}/pets/new
    [HttpPost("/owners/{ownerId}/pets/new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int ownerId, [FromForm] string name, [FromForm] DateTime birthDate, [FromForm] int type)
    {
        // Create pet from explicit parameters
        var pet = new Pet
        {
            Name = name,
            BirthDate = birthDate,
            TypeId = type,
            OwnerId = ownerId
        };
        
        var owner = await _context.Owners.Include(o => o.Pets).FirstOrDefaultAsync(o => o.Id == ownerId);
        if (owner == null)
        {
            TempData["ErrorMessage"] = "Owner not found.";
            return RedirectToAction("Index", "Owner");
        }

        // Validate birthdate not in future
        if (pet.BirthDate > DateTime.Today)
        {
            ModelState.AddModelError("BirthDate", "Birth date cannot be in the future.");
        }

        // Validate no duplicate names per owner
        if (owner.Pets.Any(p => p.Name.Equals(pet.Name, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError("Name", "This owner already has a pet with this name.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Owner = owner;
            ViewBag.PetTypes = new SelectList(await _context.PetTypes.OrderBy(pt => pt.Name).ToListAsync(), "Id", "Name", pet.TypeId);
            return View("CreateOrEdit", pet);
        }

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Pet '{pet.Name}' has been added successfully.";
        return RedirectToAction("Details", "Owner", new { id = ownerId });
    }

    // GET: /owners/{ownerId}/pets/{petId}/edit
    [HttpGet("/owners/{ownerId}/pets/{petId}/edit")]
    public async Task<IActionResult> Edit(int ownerId, int petId)
    {
        var owner = await _context.Owners.FindAsync(ownerId);
        if (owner == null)
        {
            TempData["ErrorMessage"] = "Owner not found.";
            return RedirectToAction("Index", "Owner");
        }

        var pet = await _context.Pets
            .Include(p => p.PetType)
            .FirstOrDefaultAsync(p => p.Id == petId && p.OwnerId == ownerId);

        if (pet == null)
        {
            TempData["ErrorMessage"] = "Pet not found or does not belong to this owner.";
            return RedirectToAction("Details", "Owner", new { id = ownerId });
        }

        ViewBag.Owner = owner;
        ViewBag.PetTypes = new SelectList(await _context.PetTypes.OrderBy(pt => pt.Name).ToListAsync(), "Id", "Name", pet.TypeId);
        ViewBag.IsEdit = true;

        return View("CreateOrEdit", pet);
    }

    // POST: /owners/{ownerId}/pets/{petId}/edit
    [HttpPost("/owners/{ownerId}/pets/{petId}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int ownerId, int petId, [FromForm] string name, [FromForm] DateTime birthDate, [FromForm] int type)
    {
        var owner = await _context.Owners.Include(o => o.Pets).FirstOrDefaultAsync(o => o.Id == ownerId);
        if (owner == null)
        {
            TempData["ErrorMessage"] = "Owner not found.";
            return RedirectToAction("Index", "Owner");
        }

        var existingPet = await _context.Pets.FindAsync(petId);
        if (existingPet == null || existingPet.OwnerId != ownerId)
        {
            TempData["ErrorMessage"] = "Pet not found or does not belong to this owner.";
            return RedirectToAction("Details", "Owner", new { id = ownerId });
        }

        // Validate birthdate not in future
        if (birthDate > DateTime.Today)
        {
            ModelState.AddModelError("BirthDate", "Birth date cannot be in the future.");
        }

        // Validate no duplicate names per owner (excluding current pet)
        if (owner.Pets.Any(p => p.Id != petId && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError("Name", "This owner already has another pet with this name.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Owner = owner;
            ViewBag.PetTypes = new SelectList(await _context.PetTypes.OrderBy(pt => pt.Name).ToListAsync(), "Id", "Name", type);
            ViewBag.IsEdit = true;
            var pet = new Pet { Id = petId, Name = name, BirthDate = birthDate, TypeId = type, OwnerId = ownerId };
            return View("CreateOrEdit", pet);
        }

        existingPet.Name = name;
        existingPet.BirthDate = birthDate;
        existingPet.TypeId = type;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Pet '{name}' has been updated successfully.";
        return RedirectToAction("Details", "Owner", new { id = ownerId });
    }
}
