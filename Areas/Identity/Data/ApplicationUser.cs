using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

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

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(255, ErrorMessage = "Max 255 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(255, ErrorMessage = "Max 255 characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Birth date is required.")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    [Column(TypeName = "varchar(6)")]
    public Gender Gender { get; set; }
}

