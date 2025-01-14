using ChessManager.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessManager.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TournamentId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [Range(0, 3000)]
        public int Rating { get; set; }
        public bool IsAccepted { get; set; } = false;

        [ForeignKey("TournamentId")]
        public Tournament Tournament { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
