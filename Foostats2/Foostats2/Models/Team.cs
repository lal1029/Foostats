using System.ComponentModel.DataAnnotations;

namespace Foostats2.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public string DisplayName { get; set; }
    }
}