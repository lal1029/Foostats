using System.Net;
using System.Web.Http;
using Foostats2.Metrics;
using Foostats2.Metrics.Plugins;
using Foostats2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Foostats2.Controllers
{
    public class MatchController : FoostatsController
    {
        public ActionResult GetByTeamName(String TeamDisplayName)
        {
            using (var db = new FoostatsContext())
            {
                var matches = db.Matches.
                        Include(x => x.Team1).
                        Include(x => x.Team1.Player1).
                        Include(x => x.Team1.Player2).
                        Include(x => x.Team2.Player1).
                        Include(x => x.Team2.Player2).
                        Include(x => x.Team2).Where(x =>
                    x.Team1.DisplayName.Equals(TeamDisplayName, StringComparison.InvariantCultureIgnoreCase) ||
                    x.Team2.DisplayName.Equals(TeamDisplayName, StringComparison.InvariantCultureIgnoreCase)).ToList();
                
                // convert to local time
                foreach (var match in matches)
                {
                    if (match.Team1Validated.HasValue)
                    {
                        match.Team1Validated = match.Team1Validated.Value.ToLocalTime();
                    }
                    if (match.Team2Validated.HasValue)
                    {
                        match.Team2Validated = match.Team2Validated.Value.ToLocalTime();
                    }
                }
                return View("ListMatches", matches);
            }
        }

        public ActionResult GetRivalry(String playerDisplayName)
        {
            using (var db = new FoostatsContext())
            {
                var thisPlayer = db.Players.FirstOrDefault(x => x.DisplayName == User.Identity.Name);
                if (thisPlayer == null)
                {
                    return View("ListMatches", null);
                }

                var thatPlayer = db.Players.FirstOrDefault(x => x.DisplayName == playerDisplayName);
                if (thatPlayer == null)
                {
                    return View("ListMatches", null);
                }

                var theseMatches = GetMatches(db, thisPlayer.Id);
                var thoseMatches = GetMatches(db, thatPlayer.Id);

                var matches = theseMatches.Intersect(thoseMatches).ToList();
                var matches2 = new List<Match>();

                // filter out matches where they are on the same team
                foreach (var match in matches)
                {
                    if (!((match.Team1.Player1 == thisPlayer && match.Team1.Player2 == thatPlayer) ||
                        (match.Team1.Player2 == thisPlayer && match.Team1.Player1 == thatPlayer) ||
                        (match.Team2.Player1 == thisPlayer && match.Team2.Player2 == thatPlayer) ||
                        (match.Team2.Player2 == thisPlayer && match.Team2.Player1 == thatPlayer)))
                    {
                        matches2.Add(match);
                    }
                }

                matches = matches2;

                // convert to local time
                foreach (var match in matches)
                {
                    if (match.Team1Validated.HasValue)
                    {
                        match.Team1Validated = match.Team1Validated.Value.ToLocalTime();
                    }
                    if (match.Team2Validated.HasValue)
                    {
                        match.Team2Validated = match.Team2Validated.Value.ToLocalTime();
                    }
                }

                var wins = matches.Count(x => ((x.Team1.Player1 == thisPlayer || x.Team1.Player2 == thisPlayer) && x.Score1 > x.Score2)
                                              || ((x.Team2.Player1 == thisPlayer || x.Team2.Player2 == thisPlayer) && x.Score2 > x.Score1));
                var losses = matches.Count - wins;
                ViewBag.Wins = wins;
                ViewBag.Losses = losses;
                ViewBag.OpposingPlayer = playerDisplayName;
                ViewBag.PreferredPlayer = User.Identity.Name;
                return View("ListMatches", matches);
            }
        }

        public ActionResult GetByPlayerDisplay(String playerDisplayName)
        {
            Player player = null;
            using (var db = new Foostats2.Models.FoostatsContext())
            {
                player = db.Players.FirstOrDefault(x => x.DisplayName == playerDisplayName);
                if (player == null)
                {
                    return View("ListMatches", null);
                }
                
                var matches = GetMatches(db, player.Id).ToList();

                // convert to local time
                foreach (var match in matches)
                {
                    if (match.Team1Validated.HasValue)
                    {
                        match.Team1Validated = match.Team1Validated.Value.ToLocalTime();
                    }
                    if (match.Team2Validated.HasValue)
                    {
                        match.Team2Validated = match.Team2Validated.Value.ToLocalTime();
                    }
                }

                return View("ListMatches", matches);
            }
        }

        [ValidateInput(false)]
        public ActionResult Add(String Team1Player1, String Team1Player2, String Team2Player1, String Team2Player2, int Score1, int Score2)
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<FoostatsContext>());

            Match match = null;
            Team team1, team2;
            using (var db = new FoostatsContext())
            {
                // after initialize players, Player objects are stored in t1p1, t1p2 (optional), t2p1, t2p2 (optional)
                #region Initialize Players
                // Parse out actual player names
                var startPosition = Team1Player1.IndexOf('<') + 1;
                if (startPosition != 0) // implies actual returned value is -1
                {
                    var length = Team1Player1.LastIndexOf('>') - startPosition;
                    Team1Player1 = Team1Player1.Substring(startPosition, length);
                }

                if (!string.IsNullOrEmpty(Team1Player2))
                {
                    startPosition = Team1Player2.IndexOf('<') + 1;
                    if (startPosition != 0) // implies actual returned value is -1
                    {
                        var length = Team1Player2.LastIndexOf('>') - startPosition;
                        Team1Player2 = Team1Player2.Substring(startPosition, length);
                    }
                }
                
                startPosition = Team2Player1.IndexOf('<') + 1;
                if (startPosition != 0) // implies actual returned value is -1
                {
                    var length = Team2Player1.LastIndexOf('>') - startPosition;
                    Team2Player1 = Team2Player1.Substring(startPosition, length);
                }

                if (!string.IsNullOrEmpty(Team2Player2))
                {
                    startPosition = Team2Player2.IndexOf('<') + 1;
                    if (startPosition != 0) // implies actual returned value is -1
                    {
                        var length = Team2Player2.LastIndexOf('>') - startPosition;
                        Team2Player2 = Team2Player2.Substring(startPosition, length);
                    }
                }

                // Team 1
                Player t1p1 = db.Players.FirstOrDefault(x => x.DisplayName == Team1Player1);
                if (t1p1 == null) // team 1 player 1 exists but is not created
                {
                    t1p1 = db.Players.Add(new Player()
                    {
                        DisplayName = Team1Player1
                    });
                }

                Player t1p2 = null;
                if (!String.IsNullOrEmpty(Team1Player2))
                {
                    t1p2 = db.Players.FirstOrDefault(x => x.DisplayName == Team1Player2);
                    if (t1p2 == null) // team 1 player 2 exists but is not created
                    {
                        t1p2 = db.Players.Add(new Player()
                        {
                            DisplayName = Team1Player2
                        });
                    }
                }

                // Team 2
                Player t2p1 = db.Players.FirstOrDefault(x => x.DisplayName == Team2Player1);
                if (t2p1 == null) // team 2 player 1 exists but is not created
                {
                    t2p1 = db.Players.Add(new Player()
                    {
                        DisplayName = Team2Player1
                    });
                }

                Player t2p2 = null;
                if (!String.IsNullOrEmpty(Team2Player2))
                {
                    t2p2 = db.Players.FirstOrDefault(x => x.DisplayName == Team2Player2);
                    if (t2p2 == null) // team 2 player 2 exists but is not created
                    {
                        t2p2 = db.Players.Add(new Player()
                        {
                            DisplayName = Team2Player2
                        });
                    }
                }

                db.SaveChanges();
                #endregion

                // after initialize teams, Team objects are stored in team1 and team2
                #region Initialize Teams
                if (t1p2 == null)
                {
                    team1 = db.Teams.Include(x => x.Player1).Include(x => x.Player2).FirstOrDefault(x =>
                                        (x.Player1.Id == t1p1.Id && x.Player2 == null) ||
                                        (x.Player2.Id == t1p1.Id && x.Player1 == null));
                }
                else
                {
                    team1 = db.Teams.Include(x => x.Player1).Include(x => x.Player2).FirstOrDefault(x =>
                                        (x.Player1.Id == t1p1.Id && x.Player2.Id == t1p2.Id) ||
                                        (x.Player2.Id == t1p1.Id && x.Player1.Id == t1p2.Id));
                }
                
                if (team1 == null)
                {
                    team1 = db.Teams.Add(new Team()
                    {
                        DisplayName = t1p2 == null ? t1p1.DisplayName : string.Format("{0},{1}", t1p1.DisplayName, t1p2.DisplayName),
                        Player1 = t1p1,
                        Player2 = t1p2
                    });
                }

                if (t2p2 == null)
                {
                    team2 = db.Teams.FirstOrDefault(x =>
                                        (x.Player1.Id == t2p1.Id && x.Player2 == null) ||
                                        (x.Player2.Id == t2p1.Id && x.Player1 == null));
                }
                else
                {
                    team2 = db.Teams.FirstOrDefault(x =>
                                        (x.Player1.Id == t2p1.Id && x.Player2.Id == t2p2.Id) ||
                                        (x.Player2.Id == t2p1.Id && x.Player1.Id == t2p2.Id));
                }
                
                if (team2 == null)
                {
                    team2 = db.Teams.Add(new Team()
                    {
                        DisplayName = t2p2 == null ? t2p1.DisplayName : string.Format("{0},{1}", t2p1.DisplayName, t2p2.DisplayName),
                        Player1 = t2p1,
                        Player2 = t2p2
                    });
                }

                db.SaveChanges();

                #endregion

                match = db.Matches.Add(new Match()
                {
                    Team1 = team1,
                    Team2 = team2,
                    Score1 = Score1,
                    Score2 = Score2,
                    Team1Validated = DateTime.UtcNow,
                    Team2Validated = null
                });

                db.SaveChanges();
            }

            #region Update Metrics
            foreach (var plugin in MetricCalculators)
            {
                plugin.UpdateMetrics(team1, team2, Score1, Score2);
            }
            #endregion

            return GetByTeamName(team1.DisplayName);
        }

    }
}
