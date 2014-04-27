using System.ComponentModel.DataAnnotations;

namespace Foostats2.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string MutableDisplayName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}