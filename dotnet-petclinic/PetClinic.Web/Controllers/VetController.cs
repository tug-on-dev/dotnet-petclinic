using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetClinic.Web.Data;
using PetClinic.Web.Models;
using PetClinic.Web.Models.ViewModels;

namespace PetClinic.Web.Controllers;

public class VetController : Controller
{
    private readonly PetClinicDbContext _context;
    private const int PageSize = 5;

    public VetController(PetClinicDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int page = 1)
    {
        if (page < 1) page = 1;

        var query = _context.Vets
            .Include(v => v.VetSpecialties)
            .ThenInclude(vs => vs.Specialty)
            .OrderBy(v => v.LastName)
            .ThenBy(v => v.FirstName);

        var paginatedList = PaginatedList<Vet>.Create(query, page, PageSize);

        return View(paginatedList);
    }
}
