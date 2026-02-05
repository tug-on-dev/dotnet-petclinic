# Spring PetClinic → .NET 10 Migration Plan

## Summary
Rewrite the Spring Boot PetClinic application (Java) to a .NET 10 MVC application with Razor views, Entity Framework Core, SQLite, and Bootstrap 5 styling.

## Architecture Decisions
- **Framework**: .NET 10 MVC (Monolithic)
- **UI**: Razor Pages/Views with Bootstrap 5 (default theme)
- **Database**: SQLite with Entity Framework Core
- **ORM**: Entity Framework Core (Code-First)
- **Testing**: xUnit + Playwright for E2E testing
- **Seed Data**: Same as Java app (6 vets, 10 owners, 13 pets)

---

## Source Analysis Summary

### Entities (7 tables)
| Entity | Properties | Relationships |
|--------|------------|---------------|
| Owner | Id, FirstName, LastName, Address, City, Telephone | Has many Pets |
| Pet | Id, Name, BirthDate, TypeId | Belongs to Owner, Has many Visits |
| PetType | Id, Name | Has many Pets |
| Visit | Id, PetId, VisitDate, Description | Belongs to Pet |
| Vet | Id, FirstName, LastName | Many-to-Many with Specialties |
| Specialty | Id, Name | Many-to-Many with Vets |
| VetSpecialty | VetId, SpecialtyId | Junction table |

### Screens to Implement (9 screens)
1. **Home** - Welcome page with pet image
2. **Find Owners** - Search form by last name
3. **Owners List** - Paginated search results
4. **Owner Details** - Owner info with pets and visits
5. **Create/Edit Owner** - Owner form (CRUD)
6. **Create/Edit Pet** - Pet form with type dropdown (CRUD)
7. **Create Visit** - Visit form with history display
8. **Veterinarians** - Paginated vet list with specialties
9. **Error Page** - Error handling

### Features to Implement
- [x] Navigation bar (Home, Find Owners, Veterinarians, Error)
- [ ] Pagination (5 items per page for Owners and Vets)
- [ ] Search by last name (partial match)
- [ ] Form validation with error display
- [ ] Flash messages (success/error notifications)
- [ ] Cascading relationships (Owner → Pets → Visits)

---

## Work Plan

### Phase 1: Project Setup
- [ ] 1.1 Create .NET 10 MVC project structure
- [ ] 1.2 Configure SQLite + Entity Framework Core
- [ ] 1.3 Setup Bootstrap 5 (via LibMan or CDN)
- [ ] 1.4 Create shared layout (_Layout.cshtml) with navigation

### Phase 2: Data Layer
- [ ] 2.1 Create entity models (Owner, Pet, PetType, Visit, Vet, Specialty)
- [ ] 2.2 Configure DbContext with relationships
- [ ] 2.3 Create seed data (same as Java app)
- [ ] 2.4 Run migrations and verify database

### Phase 3: Controllers & Views - Core
- [ ] 3.1 HomeController + Welcome view
- [ ] 3.2 ErrorController + Error view
- [ ] 3.3 Shared partials (_ValidationScripts, _InputField, etc.)

### Phase 4: Owner Management
- [ ] 4.1 OwnerController (CRUD operations)
- [ ] 4.2 Find Owners view (search form)
- [ ] 4.3 Owners List view (with pagination)
- [ ] 4.4 Owner Details view
- [ ] 4.5 Create/Edit Owner form with validation

### Phase 5: Pet Management
- [ ] 5.1 PetController (nested under Owner)
- [ ] 5.2 Create/Edit Pet form with type dropdown
- [ ] 5.3 Pet validation (name, date, type)

### Phase 6: Visit Management
- [ ] 6.1 VisitController (nested under Pet)
- [ ] 6.2 Create Visit form with visit history display
- [ ] 6.3 Visit validation

### Phase 7: Veterinarians
- [ ] 7.1 VetController with pagination
- [ ] 7.2 Vet List view with specialties display

### Phase 8: Polish & Validation
- [ ] 8.1 Flash messages implementation
- [ ] 8.2 Form validation styling
- [ ] 8.3 Responsive design verification
- [ ] 8.4 Error handling (404, 500)

### Phase 9: Testing
- [ ] 9.1 Setup xUnit + Playwright test project
- [ ] 9.2 E2E tests for navigation
- [ ] 9.3 E2E tests for Owner CRUD
- [ ] 9.4 E2E tests for Pet CRUD
- [ ] 9.5 E2E tests for Visit creation
- [ ] 9.6 E2E tests for Vet listing
- [ ] 9.7 E2E tests for pagination & search
- [ ] 9.8 Run tests against Java app
- [ ] 9.9 Run tests against .NET app
- [ ] 9.10 Compare results and fix discrepancies

---

## E2E Test Scenarios (Feature Parity)

| Feature | Test Case |
|---------|-----------|
| Navigation | All menu links work correctly |
| Home | Welcome page displays |
| Find Owners | Empty search returns all owners |
| Find Owners | Search by last name filters results |
| Find Owners | Single result redirects to details |
| Owners List | Pagination works (next, prev, page numbers) |
| Owner Details | Shows owner info, pets, visits |
| Create Owner | Form validation works |
| Create Owner | Success creates owner and redirects |
| Edit Owner | Pre-fills form and updates correctly |
| Create Pet | Type dropdown populated |
| Create Pet | Validation prevents duplicate names |
| Edit Pet | Pre-fills and updates correctly |
| Create Visit | Date defaults to today |
| Create Visit | Shows previous visits |
| Vet List | Shows all vets with specialties |
| Vet List | Pagination works |
| Error Page | Displays error correctly |

---

## File Structure (.NET)

```
dotnet-petclinic/
├── PetClinic.Web/
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── OwnerController.cs
│   │   ├── PetController.cs
│   │   ├── VisitController.cs
│   │   ├── VetController.cs
│   │   └── ErrorController.cs
│   ├── Models/
│   │   ├── Owner.cs
│   │   ├── Pet.cs
│   │   ├── PetType.cs
│   │   ├── Visit.cs
│   │   ├── Vet.cs
│   │   ├── Specialty.cs
│   │   └── ViewModels/
│   │       ├── OwnerSearchViewModel.cs
│   │       ├── OwnerListViewModel.cs
│   │       └── PaginationViewModel.cs
│   ├── Data/
│   │   ├── PetClinicDbContext.cs
│   │   └── DbInitializer.cs
│   ├── Views/
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml
│   │   │   ├── _ValidationScriptsPartial.cshtml
│   │   │   └── Error.cshtml
│   │   ├── Home/
│   │   │   └── Index.cshtml
│   │   ├── Owner/
│   │   │   ├── Find.cshtml
│   │   │   ├── List.cshtml
│   │   │   ├── Details.cshtml
│   │   │   └── CreateOrEdit.cshtml
│   │   ├── Pet/
│   │   │   └── CreateOrEdit.cshtml
│   │   ├── Visit/
│   │   │   └── Create.cshtml
│   │   └── Vet/
│   │       └── List.cshtml
│   ├── wwwroot/
│   │   ├── css/
│   │   ├── images/
│   │   │   └── pets.png (copy from Java)
│   │   └── lib/
│   │       └── bootstrap/
│   ├── Program.cs
│   └── appsettings.json
├── PetClinic.Tests/
│   ├── E2E/
│   │   ├── NavigationTests.cs
│   │   ├── OwnerTests.cs
│   │   ├── PetTests.cs
│   │   ├── VisitTests.cs
│   │   └── VetTests.cs
│   └── PlaywrightFixture.cs
└── PetClinic.sln
```

---

## Recent Fixes (2026-02-03)

### Issue 1: Pet Edit URL Not Working
**Problem:** URLs like `/Pet/Edit/4?ownerId=3` were not working  
**Root Cause:** Links in Owner Details used ASP.NET default routing instead of custom routes  
**Solution:** Changed links to use explicit URLs matching custom routes:
- Edit Pet: `/owners/{ownerId}/pets/{petId}/edit`
- Add Visit: `/owners/{ownerId}/pets/{petId}/visits/new`
- Add Pet: `/owners/{ownerId}/pets/new`

### Issue 2: Pets List Missing on Owner Index
**Problem:** "Pets" column showed 0 for all owners  
**Root Cause:** OwnerController.Index() didn't eager load Pets collection  
**Solution:** Added `.Include(o => o.Pets)` to query in OwnerController.Index()

**Both issues are now resolved and tested!** ✅

---

## Notes
- Use Bootstrap 5 default theme (no custom Spring colors)
- Keep validation messages consistent with Java app
- Pagination uses 1-indexed page numbers (same as Java)
- Flash messages auto-hide after 3 seconds
- Copy pets.png image from Java app for welcome page
