using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessManager.Areas.Identity.Data;

public enum Gender
{
    Male,
    Female,
    Other
}

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public override string Email { get; set; }

    [Display(Name = "Phone number")]
    public override string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(255, ErrorMessage = "Max 255 characters.")]
    [Display(Name = "First name")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(255, ErrorMessage = "Max 255 characters.")]
    [Display(Name = "Last name")]
    public string LastName { get; set; }

    [NotMapped]
    public string Name
    {
        get { return $"{FirstName} {LastName}"; }
    }

    [Required(ErrorMessage = "Birth date is required.")]
    [Display(Name = "Birth date")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    [Column(TypeName = "varchar(6)")]
    public Gender Gender { get; set; }
}

