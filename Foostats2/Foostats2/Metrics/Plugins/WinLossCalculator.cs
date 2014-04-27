using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Foostats2.Metrics.Plugins
{
    public class WinLossCalculator : IOnlineMetricCalculator
    {
        public void UpdateMetrics(Models.Team Team1, Models.Team Team2, int Score1, int Score2)
        {
            using (var dbContext = new Foostats2.Models.FoostatsContext())
            {
                Team1 = dbContext.Teams.Include(x => x.Player1).Include(x => x.Player2).SingleOrDefault(x => x.Id == Team1.Id);
                Team2 = dbContext.Teams.Include(x => x.Player1).Include(x => x.Player2).SingleOrDefault(x => x.Id == Team2.Id); // reload
                var t1p1WinLoss = dbContext.WinLoss.Include(x => x.Player).FirstOrDefault(x => x.Player.Id == Team1.Player1.Id);
                var t1p2WinLoss = Team1.Player2 == null ? null : dbContext.WinLoss.Include(x => x.Player).FirstOrDefault(x => x.Player.Id == Team1.Player2.Id);
                var t2p1WinLoss = dbContext.WinLoss.Include(x => x.Player).FirstOrDefault(x => x.Player.Id == Team2.Player1.Id);
                var t2p2WinLoss = Team2.Player2 == null ? null : dbContext.WinLoss.Include(x => x.Player).FirstOrDefault(x => x.Player.Id == Team2.Player2.Id);
                
                var t1WinsToAdd = Score1 >= Score2 ? 1 : 0;
                var t1LossToAdd = t1WinsToAdd == 1 ? 0 : 1;
                var t2WinsToAdd = Score2 >= Score1 ? 1 : 0;
                var t2LossToAdd = t2WinsToAdd == 1 ? 0 : 1;

                #region Update Team 1
                if (t1p1WinLoss != null)
                {
                    t1p1WinLoss.Win += t1WinsToAdd;
                    t1p1WinLoss.Loss += t1LossToAdd;
                }
                else
                {
                    dbContext.WinLoss.Add(new Models.WinLoss()
                    {
                        Player = Team1.Player1,
                        Win = t1WinsToAdd,
                        Loss = t1LossToAdd
                    });
                }

                if (Team1.Player2 != null)
                {
                    if (t1p2WinLoss != null)
                    {
                        t1p2WinLoss.Win += t1WinsToAdd;
                        t1p2WinLoss.Loss += t1LossToAdd;
                    }
                    else
                    {
                        dbContext.WinLoss.Add(new Models.WinLoss()
                        {
                            Player = Team1.Player2,
                            Win = t1WinsToAdd,
                            Loss = t1LossToAdd
                        });
                    }
                }
                #endregion

                #region Update Team 2
                if (t2p1WinLoss != null)
                {
                    t2p1WinLoss.Win += t2WinsToAdd;
                    t2p1WinLoss.Loss += t2LossToAdd;
                }
                else
                {
                    dbContext.WinLoss.Add(new Models.WinLoss()
                    {
                        Player = Team2.Player1,
                        Win = t2WinsToAdd,
                        Loss = t2LossToAdd
                    });
                }

                if (Team2.Player2 != null)
                {
                    if (t2p2WinLoss != null)
                    {
                        t2p2WinLoss.Win += t2WinsToAdd;
                        t2p2WinLoss.Loss += t2LossToAdd;
                    }
                    else
                    {
                        dbContext.WinLoss.Add(new Models.WinLoss()
                        {
                            Player = Team2.Player2,
                            Win = t2WinsToAdd,
                            Loss = t2LossToAdd
                        });
                    }
                }
                #endregion

                dbContext.SaveChanges();
            }
        }
    }
}