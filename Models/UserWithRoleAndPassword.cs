using System;
using System.ComponentModel.DataAnnotations;
using ChessManager.Areas.Identity.Data;

namespace ChessManager.Models
{
    public class UserWithRoleAndPassword : UserWithRole
    {
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
    }
}

