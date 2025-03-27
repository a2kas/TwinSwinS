using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwinsWins.Data.Model
{
    [Table("game_lobby")]
    public class GameLobby
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("owner_id")]
        public long OwnerId { get; set; }

        [Column("stake")]
        public decimal Stake { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }

        [Column("body")]
        public string Body { get; set; }
    }
}
