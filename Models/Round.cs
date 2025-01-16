using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessManager.Models
{
    public class Round
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TournamentId { get; set; }
        [Required]
        [Range(1, 24)]
        public int RoundNumber { get; set; }
        public TimeSpan? StartTime { get; set; }

        [ForeignKey("TournamentId")]
        public Tournament Tournament { get; set; }

        public ICollection<Match> Matches { get; set; }
    }
}
