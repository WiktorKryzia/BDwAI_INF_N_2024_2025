using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessManager.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RoundId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int BoardNumber { get; set; }
        [Required]
        public int WhitePlayerId { get; set; }
        [Required]
        public int BlackPlayerId { get; set; }
        [Column(TypeName = "varchar(7)")]
        public String? Result { get; set; } // 1-0 or 0-1 or 0.5-0.5

        [ForeignKey("RoundId")]
        public Round Round { get; set; }
        [ForeignKey("WhitePlayerId")]
        public Player WhitePlayer { get; set; }
        [ForeignKey("BlackPlayerId")]
        public Player BlackPlayer { get; set; }
    }
}
