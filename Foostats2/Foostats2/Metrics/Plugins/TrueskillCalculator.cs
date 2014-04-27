using Foostats2.Metrics;
using Moserware.Skills;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foostats2.Metrics.Plugins
{
    public class TrueskillCalculator : IOnlineMetricCalculator
    {
        public void UpdateMetrics(Models.Team Team1, Models.Team Team2, int Score1, int Score2)
        {
            using(var dbContext = new Foostats2.Models.FoostatsContext()){
                Team1 = dbContext.Teams.Include(x => x.Player1).Include(x => x.Player2).SingleOrDefault(x => x.Id == Team1.Id);
                Team2 = dbContext.Teams.Include(x => x.Player1).Include(x => x.Player2).SingleOrDefault(x => x.Id == Team2.Id); // reload

                var gameInfo = GameInfo.DefaultGameInfo;
                var t1p1Trueskill = dbContext.Trueskill.
                    Include(x => x.Player).
                    FirstOrDefault(x => x.Player.Id == Team1.Player1.Id);
                var t1p2Trueskill = Team1.Player2 == null ? null : 
                    dbContext.Trueskill.
                    Include(x => x.Player).
                    FirstOrDefault(x => x.Player.Id == Team1.Player2.Id);
                var t2p1Trueskill = dbContext.Trueskill.
                    Include(x => x.Player).
                    FirstOrDefault(x => x.Player.Id == Team2.Player1.Id);
                var t2p2Trueskill = Team2.Player2 == null ? null : 
                    dbContext.Trueskill.
                    Include(x => x.Player).
                    FirstOrDefault(x => x.Player.Id == Team2.Player2.Id);
                Rating t1p1, t1p2, t2p1, t2p2;
                t1p1 = t1p1Trueskill != null ? new Rating(t1p1Trueskill.Mean, t1p1Trueskill.StandardDeviation) : gameInfo.DefaultRating;
                t1p2 = t1p2Trueskill != null ? new Rating(t1p2Trueskill.Mean, t1p2Trueskill.StandardDeviation) : gameInfo.DefaultRating;
                t2p1 = t2p1Trueskill != null ? new Rating(t2p1Trueskill.Mean, t2p1Trueskill.StandardDeviation) : gameInfo.DefaultRating;
                t2p2 = t2p2Trueskill != null ? new Rating(t2p2Trueskill.Mean, t2p2Trueskill.StandardDeviation) : gameInfo.DefaultRating;

                var team1player1 = new Player(Team1.Player1.Id);
                var team1player2 = Team1.Player2 == null ? null : new Player(Team1.Player2.Id);
                var team2player1 = new Player(Team2.Player1.Id);
                var team2player2 = Team2.Player2 == null ? null : new Player(Team2.Player2.Id);

                var team1 = new Team();
                team1.AddPlayer(team1player1, t1p1);
                if(team1player2 != null)
                    team1.AddPlayer(team1player2, t1p2);

                var team2 = new Team();
                team2.AddPlayer(team2player1, t2p1);
                if (team2player2 != null)
                    team2.AddPlayer(team2player2, t2p2);

                var teams = Teams.Concat(team1, team2);
                
                var team1Place = Score1 >= Score2 ? 1 : 2;
                var team2Place = Score2 >= Score1 ? 1 : 2;
                var newRatings = TrueSkillCalculator.CalculateNewRatings(gameInfo, teams, team1Place, team2Place);

                var team1player1NewRating = newRatings[team1player1];
                Rating team1player2NewRating = team1player2 == null ? null : newRatings[team1player2];
                var team2player1NewRating = newRatings[team2player1];
                Rating team2player2NewRating = team2player2 == null ? null : newRatings[team2player2];


                if (t1p1Trueskill != null)
                {
                    t1p1Trueskill.Mean = team1player1NewRating.Mean;
                    t1p1Trueskill.StandardDeviation = team1player1NewRating.StandardDeviation;
                    t1p1Trueskill.ConservativeRating = team1player1NewRating.ConservativeRating;
                }
                else
                {
                    dbContext.Trueskill.Add(new Models.Trueskill()
                    {
                        Player = Team1.Player1,
                        Mean = team1player1NewRating.Mean,
                        StandardDeviation = team1player1NewRating.StandardDeviation,
                        ConservativeRating = team1player1NewRating.ConservativeRating
                    });
                }

                if (team1player2 != null)
                {
                    if (t1p2Trueskill != null)
                    {
                        t1p2Trueskill.Mean = team1player2NewRating.Mean;
                        t1p2Trueskill.StandardDeviation = team1player2NewRating.StandardDeviation;
                        t1p2Trueskill.ConservativeRating = team1player2NewRating.ConservativeRating;
                    }
                    else
                    {
                        dbContext.Trueskill.Add(new Models.Trueskill()
                        {
                            Player = Team1.Player2,
                            Mean = team1player2NewRating.Mean,
                            StandardDeviation = team1player2NewRating.StandardDeviation,
                            ConservativeRating = team1player2NewRating.ConservativeRating
                        });
                    }
                }

                if (t2p1Trueskill != null)
                {
                    t2p1Trueskill.Mean = team2player1NewRating.Mean;
                    t2p1Trueskill.StandardDeviation = team2player1NewRating.StandardDeviation;
                    t2p1Trueskill.ConservativeRating = team2player1NewRating.ConservativeRating;
                }
                else
                {
                    dbContext.Trueskill.Add(new Models.Trueskill()
                    {
                        Player = Team2.Player1,
                        Mean = team2player1NewRating.Mean,
                        StandardDeviation = team2player1NewRating.StandardDeviation,
                        ConservativeRating = team2player1NewRating.ConservativeRating
                    });
                }

                if (team2player2 != null)
                {
                    if (t2p2Trueskill != null)
                    {
                        t2p2Trueskill.Mean = team2player2NewRating.Mean;
                        t2p2Trueskill.StandardDeviation = team2player2NewRating.StandardDeviation;
                        t2p2Trueskill.ConservativeRating = team2player2NewRating.ConservativeRating;
                    }
                    else
                    {
                        dbContext.Trueskill.Add(new Models.Trueskill()
                        {
                            Player = Team2.Player2,
                            Mean = team2player2NewRating.Mean,
                            StandardDeviation = team2player2NewRating.StandardDeviation,
                            ConservativeRating = team2player2NewRating.ConservativeRating
                        });
                    }
                }

                dbContext.SaveChanges();
            }
        }
    }
}