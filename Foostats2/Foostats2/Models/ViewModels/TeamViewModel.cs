using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foostats2.Models.ViewModels
{
    public class TeamViewModel
    {
        public string Player1 { get; set; }

        public string Player2 { get; set; }

        public string TeamName { get; set; }

        public double Rating { get; set; }

    }

    public sealed class BracketNode
    {
        public BracketNode LeftNode { get ; set;}
        public BracketNode RightNode { get; set; }

        public String[] Teams { get; set; }

        public TeamViewModel leftValue { get; set; }

        public TeamViewModel rightValue { get; set; }
    }

    public sealed class MatchModel
    {
        public string[] Team1Players { get; set; }

        public string[] Team2Players { get; set; }

        public int Team1Score { get; set; }

        public int Team2Score { get; set; }
    }
}