using Foostats2.Models;
using Foostats2.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Foostats2.Utility;

namespace Foostats2.Controllers
{
    public class TournamentController : FoostatsController
    {
        //
        // GET: /Tournament/

        public ActionResult Index()
        {
            using (var db = new FoostatsContext())
            {
                var Users = db.Players.ToList().Select(x => x.MutableDisplayName != null ? string.Format("{0} <{1}>", x.MutableDisplayName, x.DisplayName) : x.DisplayName);
                ViewBag.Users = Users;
            }
            return View("Tournament");
        }

        public ActionResult CreateBracket(TeamViewModel [] models)
        {
            List<TeamViewModel> teams;
            using (var db = new FoostatsContext())
            {
               // Process all the players and get their ratings
               teams = preProcessTeams(models, db);
  
            }

            return Json(createBracket(teams , 0));
        }

        public ActionResult AddMatches(MatchModel[] matches)
        {
            using( var db = new FoostatsContext()) {

                foreach(MatchModel match in matches ) {

                    #region Initialize Players

                    //Possibly use a for loop for this later
                    Player t1p1 = getPlayer(db, match.Team1Players[0]);
                    Player t1p2 = getPlayer(db, match.Team1Players[1]);
                    Player t2p1 = getPlayer(db, match.Team2Players[0]);
                    Player t2p2 = getPlayer(db, match.Team2Players[1]);

                    #endregion

                    #region Initialize Teams
                    Team team1 = getTeam(db , t1p1 , t1p2);
                    Team team2 = getTeam(db , t2p1, t2p2);

                    #endregion

                    var newMatch = db.Matches.Add(new Match()
                    {
                        Team1 = team1,
                        Team2 = team2,
                        Score1 = match.Team1Score,
                        Score2 = match.Team2Score,
                        Team1Validated = DateTime.UtcNow,
                        Team2Validated = null
                    });

                    db.SaveChanges();
                }

            }


            return Json("success");
        }

        public static Player getPlayer( FoostatsContext db , string playerName)
        {
            Player player = null;
            if (!String.IsNullOrEmpty(playerName))
            {
                player = db.Players.FirstOrDefault(x => x.DisplayName == playerName);
            }

            return player;
        }

        public static Team getTeam(FoostatsContext db, Player player1, Player player2)
        {
            // Two player team
            Team matchTeam;
            if (player1 != null && player2 != null)
            {
                 matchTeam = db.Teams.Include(x => x.Player1).Include(x => x.Player2).FirstOrDefault(x =>
                                    (x.Player1.Id == player1.Id && x.Player2.Id == player2.Id) ||
                                    (x.Player2.Id == player1.Id && x.Player1.Id == player2.Id));

                if (matchTeam == null)
                {
                    matchTeam = db.Teams.Add(new Team()
                    {
                        DisplayName = string.Format("{0}, {1}", player1.DisplayName, player2.DisplayName),
                        Player1 = player1,
                        Player2 = player2
                    });
                }

                return matchTeam;
            }
            else
            {
                if (player2 == null)
                    {
                        matchTeam = db.Teams.Include(x => x.Player1).Include(x => x.Player2).FirstOrDefault(x =>
                                       (x.Player1.Id == player1.Id && x.Player2 == null) ||
                                       (x.Player2.Id == player1.Id && x.Player1 == null));
                    }
                    else
                    {
                        matchTeam = db.Teams.Include(x => x.Player1).Include(x => x.Player2).FirstOrDefault(x =>
                                       (x.Player1.Id == player2.Id && x.Player2 == null) ||
                                       (x.Player2.Id == player2.Id && x.Player1 == null));
                    }

                    if (matchTeam == null)
                    {
                        matchTeam = db.Teams.Add(new Team()
                        {
                            DisplayName = player1 != null? player1.DisplayName : player2.DisplayName,
                            Player1 = player1,
                            Player2 = player2
                        });
                    }

                return matchTeam;
                }
            }


        public static List<TeamViewModel> createBracket(List<TeamViewModel> teams, int algorithm)
        {
            //0 - Random except for the top two seeds
            if (algorithm == 0)
            {
                Random random = new Random();
                var secondSeed = teams[1];
                teams[1] = teams[teams.Count - 1];
                teams[teams.Count - 1] = secondSeed;

                for (int i = 0; i < 100; i++)
                {
                    int swap1 = random.Next(1, teams.Count - 1);
                    int swap2 = random.Next(1, teams.Count - 1);

                    var temp = teams[swap1];
                    teams[swap1] = teams[swap2];
                    teams[swap2] = temp;
                }
            }
            else if (algorithm == 1)
            {
                var root1 = teams.ElementAt(0);
                var root2 = teams.ElementAt(1);

                teams.Remove(root1);
                teams.Remove(root2);

                BracketNode rootNode = new BracketNode();
                rootNode.Teams = new string[2] { root1.TeamName, root2.TeamName };
                rootNode.leftValue = root1;
                rootNode.rightValue = root2;



                Queue<BracketNode> queue = new Queue<BracketNode>();
                queue.Enqueue(rootNode);

                while (queue.Count > 0)
                {
                    var node = queue.Dequeue();

                    BracketNode rightChildNode = new BracketNode();
                    rightChildNode.leftValue = node.leftValue;
                    if (teams.Count > 0)
                    {
                        var rightValue = teams.ElementAt(0);
                        teams.Remove(rightValue);
                        rightChildNode.rightValue = rightValue;
                    }
                }
                
            }

            //1  Standard Tournament generating algorithm

            //2 Complete random

            return teams;
        }

        public List<TeamViewModel> preProcessTeams ( TeamViewModel [] teams, FoostatsContext db)
        {
            List<TeamViewModel> models = new List<TeamViewModel>();

            foreach (TeamViewModel team in teams)
            {
                // Process all the players and get their ratings
                Player t1p1 = null;
                if (!String.IsNullOrEmpty(team.Player1))
                {
                    String player1Name = FoosHelpers.getPlayerName(team.Player1);
                    // May need to fix this
                    t1p1 = db.Players.FirstOrDefault(x => x.DisplayName == player1Name);
                    if (t1p1 == null) 
                    {
                        t1p1 = db.Players.Add(new Player()
                        {
                            DisplayName = player1Name
                        });
                    }
                    var trueskill1 = db.Trueskill.Include(x => x.Player).FirstOrDefault(x => x.Player.DisplayName == t1p1.DisplayName);
                    if (trueskill1 != null)
                    {
                        team.Rating += trueskill1.ConservativeRating;
                    }
                    else
                    {
                        team.Rating += 0;
                    }
                }

                Player t1p2 = null;
                if (!String.IsNullOrEmpty(team.Player2))
                {
                    String player2Name = FoosHelpers.getPlayerName(team.Player2);
                    t1p2 = db.Players.FirstOrDefault(x => x.DisplayName == player2Name);
                    if (t1p2 == null) // team 1 player 1 exists but is not created
                    {
                        t1p2 = db.Players.Add(new Player()
                        {
                            DisplayName = player2Name
                        });
                    }
                    var trueskill1 = db.Trueskill.Include(x => x.Player).FirstOrDefault(x => x.Player.DisplayName == t1p2.DisplayName);
                    if (trueskill1 != null)
                    {
                        team.Rating += trueskill1.ConservativeRating;
                    }
                    else
                    {
                        team.Rating += 0;
                    }
                }

                // Two player team
                if (t1p1 != null && t1p2 != null)
                {
                    Team matchTeam = db.Teams.Include(x => x.Player1).Include(x => x.Player2).FirstOrDefault(x =>
                                        (x.Player1.Id == t1p1.Id && x.Player2.Id == t1p2.Id) ||
                                        (x.Player2.Id == t1p1.Id && x.Player1.Id == t1p2.Id));

                    if (matchTeam == null)
                    {
                        matchTeam = db.Teams.Add(new Team()
                        {
                            DisplayName = string.Format("{0}, {1}", t1p1.DisplayName, t1p2.DisplayName),
                            Player1 = t1p1,
                            Player2 = t1p2
                        });
                    }
                }

                else
                {
                    Team matchTeam;
                    if (t1p2 == null)
                    {
                        matchTeam = db.Teams.Include(x => x.Player1).Include(x => x.Player2).FirstOrDefault(x =>
                                       (x.Player1.Id == t1p1.Id && x.Player2 == null) ||
                                       (x.Player2.Id == t1p1.Id && x.Player1 == null));
                    }
                    else
                    {
                        matchTeam = db.Teams.Include(x => x.Player1).Include(x => x.Player2).FirstOrDefault(x =>
                                       (x.Player1.Id == t1p2.Id && x.Player2 == null) ||
                                       (x.Player2.Id == t1p2.Id && x.Player1 == null));
                    }

                    if (matchTeam == null)
                    {
                        matchTeam = db.Teams.Add(new Team()
                        {
                            DisplayName = t1p1 != null? t1p1.DisplayName : t1p2.DisplayName,
                            Player1 = t1p1,
                            Player2 = t1p2
                        });
                    }
                }

                models.Add(team);

            }

            if(models.Count % 2 != 0)
            {
                TeamViewModel byeTeam = new TeamViewModel();
                byeTeam.TeamName = "Bye";
                byeTeam.Player1 = String.Empty;
                byeTeam.Player2 = String.Empty;
                byeTeam.Rating = 0;
                models.Add(byeTeam);
            }

           return models.OrderByDescending(x => x.Rating).ToList();
        }



    }
}
