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
        [Range(1, int.MaxValue, ErrorMessage = "Maximum number of players must be greater than 0.")]
        [Display(Name = "Max players")]
        public int MaxPlayers { get; set; }
        [Required]
        [Display(Name = "Start date")]
        public DateOnly StartDate { get; set; }
        [Required]
        [Display(Name = "End date")]
        public DateOnly EndDate { get; set; }
        [Required]
        [Display(Name = "Creation date")]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        [Required]
        [Display(Name = "Last update")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        [Required]
        public string ArbiterId { get; set; }

        [ForeignKey("ArbiterId")]
        public ApplicationUser? Arbiter { get; set; }

        public ICollection<Player> Players { get; } = new List<Player>();
        public ICollection<Round> Rounds { get; } = new List<Round>();

        [NotMapped]
        public int PlayerCount
        {
            get { return Players?.Count ?? 0; }
        }

        [NotMapped]
        public string Attendance
        {
            get { return $"{PlayerCount}/{MaxPlayers}"; }
        }
    }
}