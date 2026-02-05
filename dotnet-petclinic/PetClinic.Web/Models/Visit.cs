using System.ComponentModel.DataAnnotations;

namespace PetClinic.Web.Models;

public class Visit
{
    public int Id { get; set; }

    [Required]
    public int PetId { get; set; }

    [Required]
    public DateTime VisitDate { get; set; }

    [Required]
    [StringLength(255)]
    public string Description { get; set; } = string.Empty;

    public Pet Pet { get; set; } = null!;
}
