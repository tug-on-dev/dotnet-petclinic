using System.ComponentModel.DataAnnotations;

namespace PetClinic.Web.Models;

public class Pet
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime BirthDate { get; set; }

    [Required]
    public int OwnerId { get; set; }

    [Required]
    public int TypeId { get; set; }

    public Owner Owner { get; set; } = null!;
    public PetType PetType { get; set; } = null!;

    public List<Visit> Visits { get; set; } = new();
}
