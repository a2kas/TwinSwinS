using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwinsWins.Data.Model
{
    [Table("game_transaction")]
    public class GameTransaction
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("owner_id")]
        public long OwnerId { get; set; }

        [Column("opponent_id")]
        public long OpponentId { get; set; }

        [Column("owner_score")]
        public int OwnerScore { get; set; }

        [Column("opponent_score")]
        public int OpponentScore { get; set; }

        [Column("stake")]
        public decimal Stake { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }

        [Column("transaction")]
        public string transaction { get; set; }
    }
}
