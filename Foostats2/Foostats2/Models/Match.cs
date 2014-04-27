using System;
using System.ComponentModel.DataAnnotations;

namespace Foostats2.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public DateTime? Team1Validated { get; set; }
        public DateTime? Team2Validated { get; set; }
    }
}