using ChessManager.Areas.Identity.Data;

namespace ChessManager.Models
{
    public class UserWithRole : ApplicationUser
    {
        public string Role { get; set; }
    }
}
