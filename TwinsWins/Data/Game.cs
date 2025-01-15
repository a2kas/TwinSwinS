using System.ComponentModel.DataAnnotations.Schema;

namespace TwinsWins.Data
{
    [Table("Game")]
    public class Game
    {
        public long OwnerId { get; set; }
        public int OwnerScore { get; set; }
        public long OpponentId { get; set; }
        public int OpponentScore { get; set; }
        public decimal Stake { get; set; }
        public long WinnerId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Completed { get; set; }
    }
}
