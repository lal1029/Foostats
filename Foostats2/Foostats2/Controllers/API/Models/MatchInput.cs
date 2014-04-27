using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foostats2.Controllers.API
{
    public class MatchInput
    {
        public string Team1Player1Alias { get; set; }
        public string Team1Player2Alias { get; set; }
        public string Team2Player1Alias { get; set; }
        public string Team2Player2Alias { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
    }
}