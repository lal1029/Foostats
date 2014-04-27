using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Foostats2.Models;
using Foostats2.Utility;

namespace Foostats2.Controllers
{
    public class PlayerController : FoostatsController
    {
        private class Date
        {
            public Date(int year, int month, int day)
            {
                this.Year = year;
                this.Month = month;
                this.Day = day;
            }

            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }

            public override int GetHashCode()
            {
                return this.Year * 10000 + this.Month * 100 + this.Day;
            }

            public override bool Equals(object obj)
            {
                Date d = obj as Date;
                if (d == null)
                {
                    return false;
                }
                return d.Year == this.Year && d.Month == this.Month && d.Day == this.Day;
            }
        }

        private class KeyValue
        {
            public Date Key { get; set; }
            public int Value { get; set; }

            public KeyValue(Date key, int value)
            {
                this.Key = key;
                this.Value = value;
            }
        }

        public ActionResult MatchPerDay(string player)
        {
            using (var db = new Foostats2.Models.FoostatsContext())
            {
                var playerObj = db.Players.FirstOrDefault(x => x.DisplayName == player);
                if (playerObj == null){
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                var matchesList = GetMatches(db, playerObj.Id).ToList();

                // convert to local time
                foreach (var match in matchesList)
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

                var matches = matchesList.Select(x => new Date(x.Team1Validated.Value.Year, x.Team1Validated.Value.Month, x.Team1Validated.Value.Day));
                Dictionary<Date, int> matchesPerDay = new Dictionary<Date,int>();
                foreach (var match in matches)
                {
                    var m = 0;
                    var hash = match.GetHashCode();
                    matchesPerDay.TryGetValue(match, out m);
                    matchesPerDay[match] = m += 1;
                }
                List<KeyValue> returnValues = new List<KeyValue>();
                foreach (var date in matchesPerDay)
                {
                    returnValues.Add(new KeyValue(date.Key, date.Value));
                }
                return Json(returnValues, JsonRequestBehavior.AllowGet);
            }
        }

        public new ActionResult Profile(string playerDisplayName)
        {
            Player player = null;
            using (var db = new Foostats2.Models.FoostatsContext())
            {
                player = db.Players.FirstOrDefault(x => x.DisplayName == playerDisplayName);
                if (player == null)
                {
                    return View("Profile", null);
                }

                ViewBag.WinLoss = db.WinLoss.Include(x => x.Player).
                    FirstOrDefault(x => x.Player.DisplayName == playerDisplayName);
                ViewBag.Trueskill = db.Trueskill.Include(x => x.Player).
                    FirstOrDefault(x => x.Player.DisplayName == playerDisplayName);
                var matches = GetMatches(db, player.Id).Take(10).ToList();
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

                ViewBag.matches = matches;
            }

            var fullName = playerDisplayName.Split('\\');
            var domain = fullName[0];
            var alias = fullName[1];
            //domain = "Redmond";
            //var path = ImageHelper.GetImage(domain, alias);
            var ImageFormats = new List<string>() { "jpg", "png", "gif" };
            var RootAvatarFolder = "~/Images/users/";

            foreach (var format in ImageFormats)
            {
                var stringPath = String.Format(
                                        "{0}{1}.{2}",
                                        RootAvatarFolder,
                                        alias,
                                        format);
                string filePath = Request.MapPath(stringPath);
                if (System.IO.File.Exists(filePath))
                {
                    ViewBag.Avatar = stringPath;
                    break;
                }
            }

            if (ViewBag.Avatar == null)
            {
                ViewBag.Avatar = "~/Images/users/MissingNo.png";
            }

            return View("Profile", player);
        }

        public ActionResult List()
        {
            using (var db = new FoostatsContext())
            {
                List<string> players = db.Players.Select(x => x.DisplayName).ToList();
                return Json(players, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SetMutableDisplayName(string mutableDisplayName)
        {
            using (var db = new FoostatsContext())
            {
                var player = db.Players.FirstOrDefault(x => x.DisplayName == User.Identity.Name);
                if (player != null)
                {
                    player.MutableDisplayName = mutableDisplayName;
                    db.SaveChanges();
                }
            }

            return Profile(User.Identity.Name);
        }

        public ActionResult TopPlayers()
        {
            using (var db = new FoostatsContext())
            {
                var players = db.Trueskill.Include(x => x.Player).OrderByDescending(x => x.ConservativeRating).ToList();
                return View(players);
            }
        }
    }
}
