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
    [Required]
    [MaxLength(255)]
    public required string FirstName { get; set; }

    [Required]
    [MaxLength(255)]
    public required string LastName { get; set; }

    [Required]
    public required DateTime BirthDate { get; set; }

    [Required]
    [Column(TypeName = "varchar(6)")]
    public required Gender Gender { get; set; }
}

