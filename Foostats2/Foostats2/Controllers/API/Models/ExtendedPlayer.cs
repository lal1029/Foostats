using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Foostats2.Models;

namespace Foostats2.Controllers.API
{
    public class ExtendedPlayer : Player
    {
        public Trueskill Trueskill { get; set; }
        public WinLoss WinLoss { get; set; }
    }
}