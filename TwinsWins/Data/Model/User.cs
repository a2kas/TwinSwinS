using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwinsWins.Data.Model
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("blocked")]
        public bool blocked { get; set; }
    }
}
