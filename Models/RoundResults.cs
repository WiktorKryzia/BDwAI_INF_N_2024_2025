namespace ChessManager.Models
{

    public class RoundResults
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Rating { get; set; }
        public double TotalPoints { get; set; }
    }

    public class RoundResultsViewData
    {
        public int TournamentId { get; set; }
        public int RoundNumber { get; set; }
        public int CurrentRound { get; set; }
        public ICollection<RoundResults> Results { get; set; }
    }
}
