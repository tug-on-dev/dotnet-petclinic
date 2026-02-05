using System.ComponentModel.DataAnnotations;

namespace PetClinic.Web.Models;

public class Owner
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string City { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Telephone must be 10 digits")]
    [StringLength(20)]
    public string Telephone { get; set; } = string.Empty;

    public List<Pet> Pets { get; set; } = new();
}
