using PetClinic.Web.Models;

namespace PetClinic.Web.Data;

public static class DbInitializer
{
    public static void Initialize(PetClinicDbContext context)
    {
        context.Database.EnsureCreated();

        // Check if database is already seeded
        if (context.PetTypes.Any())
        {
            return;
        }

        // Seed PetTypes
        var petTypes = new PetType[]
        {
            new() { Name = "cat" },
            new() { Name = "dog" },
            new() { Name = "lizard" },
            new() { Name = "snake" },
            new() { Name = "bird" },
            new() { Name = "hamster" }
        };
        context.PetTypes.AddRange(petTypes);
        context.SaveChanges();

        // Seed Specialties
        var specialties = new Specialty[]
        {
            new() { Name = "radiology" },
            new() { Name = "surgery" },
            new() { Name = "dentistry" }
        };
        context.Specialties.AddRange(specialties);
        context.SaveChanges();

        // Seed Vets
        var vets = new Vet[]
        {
            new() { FirstName = "James", LastName = "Carter" },
            new() { FirstName = "Helen", LastName = "Leary" },
            new() { FirstName = "Linda", LastName = "Douglas" },
            new() { FirstName = "Rafael", LastName = "Ortega" },
            new() { FirstName = "Henry", LastName = "Stevens" },
            new() { FirstName = "Sharon", LastName = "Jenkins" }
        };
        context.Vets.AddRange(vets);
        context.SaveChanges();

        // Seed Vet Specialties
        var vetSpecialties = new VetSpecialty[]
        {
            new() { VetId = vets[1].Id, SpecialtyId = specialties[0].Id }, // Helen Leary - radiology
            new() { VetId = vets[2].Id, SpecialtyId = specialties[1].Id }, // Linda Douglas - surgery
            new() { VetId = vets[2].Id, SpecialtyId = specialties[2].Id }, // Linda Douglas - dentistry
            new() { VetId = vets[3].Id, SpecialtyId = specialties[1].Id }, // Rafael Ortega - surgery
            new() { VetId = vets[4].Id, SpecialtyId = specialties[0].Id }  // Henry Stevens - radiology
        };
        context.VetSpecialties.AddRange(vetSpecialties);
        context.SaveChanges();

        // Seed Owners
        var owners = new Owner[]
        {
            new() { FirstName = "George", LastName = "Franklin", Address = "110 W. Liberty St.", City = "Madison", Telephone = "6085551023" },
            new() { FirstName = "Betty", LastName = "Davis", Address = "638 Cardinal Ave.", City = "Sun Prairie", Telephone = "6085551749" },
            new() { FirstName = "Eduardo", LastName = "Rodriquez", Address = "2693 Commerce St.", City = "McFarland", Telephone = "6085558763" },
            new() { FirstName = "Harold", LastName = "Davis", Address = "563 Friendly St.", City = "Windsor", Telephone = "6085553198" },
            new() { FirstName = "Peter", LastName = "McTavish", Address = "2387 S. Fair Way", City = "Madison", Telephone = "6085552765" },
            new() { FirstName = "Jean", LastName = "Coleman", Address = "105 N. Lake St.", City = "Monona", Telephone = "6085552654" },
            new() { FirstName = "Jeff", LastName = "Black", Address = "1450 Oak Blvd.", City = "Monona", Telephone = "6085555387" },
            new() { FirstName = "Maria", LastName = "Escobito", Address = "345 Maple St.", City = "Madison", Telephone = "6085557683" },
            new() { FirstName = "David", LastName = "Schroeder", Address = "2749 Blackhawk Trail", City = "Madison", Telephone = "6085559435" },
            new() { FirstName = "Carlos", LastName = "Estaban", Address = "2335 Independence La.", City = "Waunakee", Telephone = "6085555487" }
        };
        context.Owners.AddRange(owners);
        context.SaveChanges();

        // Seed Pets
        var pets = new Pet[]
        {
            new() { Name = "Leo", BirthDate = new DateTime(2010, 9, 7), OwnerId = owners[0].Id, TypeId = petTypes[0].Id },
            new() { Name = "Basil", BirthDate = new DateTime(2012, 8, 6), OwnerId = owners[1].Id, TypeId = petTypes[5].Id },
            new() { Name = "Rosy", BirthDate = new DateTime(2011, 4, 17), OwnerId = owners[2].Id, TypeId = petTypes[1].Id },
            new() { Name = "Jewel", BirthDate = new DateTime(2010, 3, 7), OwnerId = owners[2].Id, TypeId = petTypes[1].Id },
            new() { Name = "Iggy", BirthDate = new DateTime(2010, 11, 30), OwnerId = owners[3].Id, TypeId = petTypes[2].Id },
            new() { Name = "George", BirthDate = new DateTime(2010, 1, 20), OwnerId = owners[4].Id, TypeId = petTypes[3].Id },
            new() { Name = "Samantha", BirthDate = new DateTime(2012, 9, 4), OwnerId = owners[5].Id, TypeId = petTypes[0].Id },
            new() { Name = "Max", BirthDate = new DateTime(2012, 9, 4), OwnerId = owners[5].Id, TypeId = petTypes[0].Id },
            new() { Name = "Lucky", BirthDate = new DateTime(2011, 8, 6), OwnerId = owners[6].Id, TypeId = petTypes[4].Id },
            new() { Name = "Mulligan", BirthDate = new DateTime(2007, 2, 24), OwnerId = owners[7].Id, TypeId = petTypes[1].Id },
            new() { Name = "Freddy", BirthDate = new DateTime(2010, 3, 9), OwnerId = owners[8].Id, TypeId = petTypes[4].Id },
            new() { Name = "Lucky", BirthDate = new DateTime(2010, 6, 24), OwnerId = owners[9].Id, TypeId = petTypes[1].Id },
            new() { Name = "Sly", BirthDate = new DateTime(2012, 6, 8), OwnerId = owners[9].Id, TypeId = petTypes[0].Id }
        };
        context.Pets.AddRange(pets);
        context.SaveChanges();

        // Seed Visits
        var visits = new Visit[]
        {
            new() { PetId = pets[6].Id, VisitDate = new DateTime(2013, 1, 1), Description = "rabies shot" },
            new() { PetId = pets[7].Id, VisitDate = new DateTime(2013, 1, 2), Description = "rabies shot" },
            new() { PetId = pets[7].Id, VisitDate = new DateTime(2013, 1, 3), Description = "neutered" },
            new() { PetId = pets[9].Id, VisitDate = new DateTime(2013, 1, 4), Description = "spayed" }
        };
        context.Visits.AddRange(visits);
        context.SaveChanges();
    }
}
