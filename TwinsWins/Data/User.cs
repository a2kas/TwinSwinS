using System.ComponentModel.DataAnnotations.Schema;

namespace TwinsWins.Data
{
    [Table("User")]
    public class User
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string Address { get; set; }
    }
}
