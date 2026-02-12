using Microsoft.EntityFrameworkCore;
using PetClinic.Web.Models;

namespace PetClinic.Web.Data;

public class PetClinicDbContext : DbContext
{
    public PetClinicDbContext(DbContextOptions<PetClinicDbContext> options) : base(options)
    {
    }

    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<PetType> PetTypes => Set<PetType>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<Vet> Vets => Set<Vet>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<VetSpecialty> VetSpecialties => Set<VetSpecialty>();
    public DbSet<Drug> Drugs => Set<Drug>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names to snake_case
        modelBuilder.Entity<Owner>().ToTable("owners");
        modelBuilder.Entity<Pet>().ToTable("pets");
        modelBuilder.Entity<PetType>().ToTable("pet_types");
        modelBuilder.Entity<Visit>().ToTable("visits");
        modelBuilder.Entity<Vet>().ToTable("vets");
        modelBuilder.Entity<Specialty>().ToTable("specialties");
        modelBuilder.Entity<VetSpecialty>().ToTable("vet_specialties");
        modelBuilder.Entity<Drug>().ToTable("drugs");

        // Configure Owner -> Pets relationship with cascade delete
        modelBuilder.Entity<Owner>()
            .HasMany(o => o.Pets)
            .WithOne(p => p.Owner)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure PetType -> Pets relationship with restrict delete
        modelBuilder.Entity<PetType>()
            .HasMany(pt => pt.Pets)
            .WithOne(p => p.PetType)
            .HasForeignKey(p => p.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Pet -> Visits relationship with cascade delete
        modelBuilder.Entity<Pet>()
            .HasMany(p => p.Visits)
            .WithOne(v => v.Pet)
            .HasForeignKey(v => v.PetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Vet <-> Specialty many-to-many relationship
        modelBuilder.Entity<VetSpecialty>()
            .HasKey(vs => new { vs.VetId, vs.SpecialtyId });

        modelBuilder.Entity<VetSpecialty>()
            .HasOne(vs => vs.Vet)
            .WithMany(v => v.VetSpecialties)
            .HasForeignKey(vs => vs.VetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VetSpecialty>()
            .HasOne(vs => vs.Specialty)
            .WithMany(s => s.VetSpecialties)
            .HasForeignKey(vs => vs.SpecialtyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
