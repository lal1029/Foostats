using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Foostats2.Models
{
    public class Trueskill
    {
        [Key]
        public int Id { get; set; }
        public Player Player { get; set; }
        public double StandardDeviation { get; set; }
        public double Mean { get; set; }
        public double ConservativeRating { get; set; }
    }
}