using ChessManager.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessManager.Models
{
    public class Tournament
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Location { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "MaxPlayers must be greater than 0.")]
        public int MaxPlayers { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        [Required]
        public string ArbiterId { get; set; }

        [ForeignKey("ArbiterId")]
        public ApplicationUser Arbiter { get; set; }
        public ICollection<Player> Players { get; set; }
    }
}