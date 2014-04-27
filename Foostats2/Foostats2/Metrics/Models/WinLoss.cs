using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foostats2.Models
{
    public class WinLoss
    {
        [Key]
        public int Id { get; set; }
        public Player Player { get; set; }
        public int Win { get; set; }
        public int Loss { get; set; }
    }
}