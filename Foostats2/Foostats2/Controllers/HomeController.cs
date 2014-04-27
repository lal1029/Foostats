using Foostats2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace Foostats2.Controllers
{
    public class HomeController : FoostatsController
    {
        public ActionResult Index()
        {
            using(var db = new FoostatsContext()){
                var players = db.Trueskill.Include(x => x.Player).OrderByDescending(x => x.ConservativeRating).Take(10).ToList();
                ViewBag.Players = players;
                ViewBag.Users = db.Players.ToList().Select(x => x.MutableDisplayName != null ? string.Format("{0} <{1}>", x.MutableDisplayName ,x.DisplayName) : x.DisplayName);
            }
            
            return View();
        }

        public ActionResult Recalculate()
        {
            using (var db = new FoostatsContext())
            {
                var matches = db.Matches.Include(x => x.Team1).Include(x => x.Team2).OrderBy((x) => x.Team1Validated);
                foreach (var match in matches)
                {
                    foreach (var plugin in MetricCalculators)
                    {
                        plugin.UpdateMetrics(match.Team1, match.Team2, match.Score1, match.Score2);
                    }
                }
            }

            return View();
        }
    }
}
