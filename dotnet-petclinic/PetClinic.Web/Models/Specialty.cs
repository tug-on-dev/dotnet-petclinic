using System.ComponentModel.DataAnnotations;

namespace PetClinic.Web.Models;

public class Specialty
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    public List<VetSpecialty> VetSpecialties { get; set; } = new();
}
