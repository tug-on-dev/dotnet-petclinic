using System.ComponentModel.DataAnnotations;

namespace PetClinic.Web.Models;

public class Vet
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    public List<VetSpecialty> VetSpecialties { get; set; } = new();
}
