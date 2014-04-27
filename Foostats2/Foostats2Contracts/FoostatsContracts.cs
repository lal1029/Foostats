using System;
using System.Runtime.Serialization;

namespace Foostats2Contracts
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Foostats2.Controllers.API")]
    public class MatchInput
    {
        [DataMember]
        public string Team1Player1Alias { get; set; }

        [DataMember]
        public string Team1Player2Alias { get; set; }

        [DataMember]
        public string Team2Player1Alias { get; set; }

        [DataMember]
        public string Team2Player2Alias { get; set; }

        [DataMember]
        public int Team1Score { get; set; }
        
        [DataMember]
        public int Team2Score { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Foostats2.Controllers.API")]
    public class ListAllInput
    {
        [DataMember]
        public string OrderBy { get; set; }

        [DataMember]
        public bool Desc { get; set; }

        [DataMember]
        public int Limit { get; set; }

        [DataMember]
        public int StartIndex { get; set; }

        [DataMember]
        public bool IncludeExtendedData { get; set; }

        [DataMember]
        public string SearchKey { get; set; }

        [DataMember]
        public string SearchTerm { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Foostats2.Controllers.API")]
    public class ExtendedPlayer : Player
    {
        [DataMember]
        public Trueskill Trueskill { get; set; }
        
        [DataMember]
        public WinLoss WinLoss { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Foostats2.Models")]
    public class Player
    {
        [DataMember]
        public int Id { get; set; }
        
        [DataMember(Name = "DisplayName")]
        public string Alias { get; set; }
        
        [DataMember]
        public string MutableDisplayName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Salt { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Foostats2.Models")]
    public class Trueskill
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Player Player { get; set; }

        [DataMember]
        public double StandardDeviation { get; set; }

        [DataMember]
        public double Mean { get; set; }

        [DataMember]
        public double ConservativeRating { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Foostats2.Models")]
    public class WinLoss
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Player Player { get; set; }

        [DataMember]
        public int Win { get; set; }

        [DataMember]
        public int Loss { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Foostats2.Models")]
    public class Match
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Team Team1 { get; set; }

        [DataMember]
        public Team Team2 { get; set; }

        [DataMember]
        public int Score1 { get; set; }

        [DataMember]
        public int Score2 { get; set; }

        [DataMember]
        public DateTime? Team1Validated { get; set; }

        [DataMember]
        public DateTime? Team2Validated { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Foostats2.Models")]
    public class Team
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Player Player1 { get; set; }

        [DataMember]
        public Player Player2 { get; set; }

        [DataMember]
        public string DisplayName { get; set; }
    }
}
