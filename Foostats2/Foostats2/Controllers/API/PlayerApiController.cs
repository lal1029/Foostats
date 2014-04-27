using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Foostats2.Controllers.API;
using Foostats2.Models;
using System.Data.Entity;

namespace Foostats2.Controllers
{
    /// <summary>
    /// Player centric operations (aggregates trueskill and winloss)
    /// 
    /// Some of the logic around merging player and trueskill/winloss data
    /// is suboptimal, but should be okay for a limited dataset. For larger
    /// datasets we may want to look at some ways to let the database do
    /// more of the join functionality.
    /// </summary>
    public class PlayerApiController : FoostatsApiController
    {
        /// <summary>
        /// Do not go over this many ListAll results, prevents long requests 
        /// from happening. If you need more than 1000 results use List
        /// with StartIndex and Limit
        /// </summary>
        private const int MaxListAll = 1000;

        /// <summary>
        /// Returns a list of all users. Limits the result to 1000 results.
        /// </summary>
        /// <returns>A list of all players</returns>
        [HttpGet]
        public IEnumerable<ExtendedPlayer> ListAll()
        {
            using (var db = new FoostatsContext())
            {
                var players = db.Players.Take(MaxListAll).Select(x => x as ExtendedPlayer).ToList(); // cast it because it is anyway :/
                return players;
            }
        }

        /// <summary>
        /// Returns a player based on the given alias. Player data is stripped 
        /// from winloss and trueskill to avoid duplication.
        /// </summary>
        /// <param name="alias">Alias of the player to get data for</param>
        /// <returns>Player with trueskill and winloss data or throws a 404 not found.</returns>
        [HttpGet]
        public ExtendedPlayer GetPlayerByAlias(string alias)
        {
            using (var db = new FoostatsContext())
            {
                var player = db.Players.First(x => x.DisplayName == alias);
                if (player == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            ReasonPhrase = string.Format("Player '{0}' could not be found.", alias)
                        });
                }
                var trueskill = db.Trueskill.Include(x => x.Player).First(x => x.Player.DisplayName == alias);
                trueskill.Player = null;
                var winloss = db.WinLoss.Include(x => x.Player).First(x => x.Player.DisplayName == alias);
                winloss.Player = null;
                var extendedPlayer = new ExtendedPlayer()
                {
                    Id = player.Id,
                    DisplayName = player.DisplayName,
                    MutableDisplayName = player.MutableDisplayName,
                    Password = player.Password,
                    Salt = player.Salt,
                    Trueskill = trueskill,
                    WinLoss = winloss
                };
                return extendedPlayer;
            }
        }

        /// <summary>
        /// Returns a list of players with the matching display name. Display
        /// name (the mutable display name) is not unique therefore this returns
        /// a list of results. Player data is stripped from winloss and trueskill
        /// to avoid duplication.
        /// </summary>
        /// <param name="displayName">Display name of the player to get data for</param>
        /// <returns>List of players with trueskill and winloss data or empty list.</returns>
        [HttpGet]
        public IEnumerable<ExtendedPlayer> GetPlayerByDisplayName(string displayName)
        {
            using (var db = new FoostatsContext())
            {
                var players = db.Players.Where(x => x.MutableDisplayName == displayName);
                var trueskill = db.Trueskill.Include(x => x.Player).Where(x => x.Player.MutableDisplayName == displayName);
                var winloss = db.WinLoss.Include(x => x.Player).Where(x => x.Player.MutableDisplayName == displayName);
                List<ExtendedPlayer> extendedPlayers = new List<ExtendedPlayer>();
                foreach (var player in players)
                {
                    var thisTrueSkill = trueskill.FirstOrDefault(x => x.Player.Id == player.Id);
                    if(thisTrueSkill != null)
                        thisTrueSkill.Player = null;
                    var thisWinLoss = winloss.FirstOrDefault(x => x.Player.Id == player.Id);
                    if(thisWinLoss != null)
                        thisWinLoss.Player = null;
                    extendedPlayers.Add(new ExtendedPlayer()
                        {
                            Id = player.Id,
                            DisplayName = player.DisplayName,
                            MutableDisplayName = player.MutableDisplayName,
                            Password = player.Password,
                            Salt = player.Salt,
                            Trueskill = thisTrueSkill,
                            WinLoss = thisWinLoss
                        });
                }
                return extendedPlayers;
            }
        }
        
        /// <summary>
        /// Returns a list of all users that match the given search. See the
        /// readme for description of input and behavior.
        /// </summary>
        /// <param name="input">Search paremeters</param>
        /// <returns>A list of users</returns>
        [HttpPost]
        [ActionName("List")]
        public IEnumerable<ExtendedPlayer> ListAllOrderedBy(ListAllInput input)
        {
            using (var db = new FoostatsContext())
            {
                var players = db.Players;
                IEnumerable<ExtendedPlayer> extendedPlayers = null;

                List<Trueskill> trueskill = null;
                List<WinLoss> winloss = null;
                if (input.IncludeExtendedData)
                {
                    trueskill = db.Trueskill.Include(x => x.Player).ToList();
                    winloss = db.WinLoss.Include(x => x.Player).ToList();
                }

                IQueryable<Player> queryAble = null;
                if (string.Compare(input.OrderBy, "DisplayName", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    queryAble = input.Desc ? players.OrderByDescending(x => x.MutableDisplayName) : players.OrderBy(x => x.MutableDisplayName);
                }
                else if (string.Compare(input.OrderBy, "Alias", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    queryAble = input.Desc ? players.OrderByDescending(x => x.DisplayName) : players.OrderBy(x => x.DisplayName);
                }
                else if (string.Compare(input.OrderBy, "Id", StringComparison.InvariantCultureIgnoreCase) == 0 || 
                    string.IsNullOrEmpty(input.OrderBy))
                {
                    queryAble = input.Desc ? players.OrderByDescending(x => x.Id) : players.OrderBy(x => x.Id);
                }
                else if (input.IncludeExtendedData && string.Compare(input.OrderBy, "Trueskill", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    // have to match first
                    extendedPlayers = players.ToList().Select(x => new ExtendedPlayer()
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName,
                        MutableDisplayName = x.MutableDisplayName,
                        Password = x.Password,
                        Salt = x.Salt,
                        Trueskill = trueskill.FirstOrDefault(y => y.Player.Id == x.Id),
                        WinLoss = winloss.FirstOrDefault(z => z.Player.Id == x.Id)
                    });
                    extendedPlayers = input.Desc ? extendedPlayers.OrderByDescending(x => x.Trueskill == null ? 0 : x.Trueskill.ConservativeRating) :
                        extendedPlayers.OrderBy(x => x => x.Trueskill == null ? 0 : x.Trueskill.ConservativeRating);
                }
                else if (input.IncludeExtendedData && string.Compare(input.OrderBy, "Trueskill", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    // have to match first
                    extendedPlayers = players.ToList().Select(x => new ExtendedPlayer()
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName,
                        MutableDisplayName = x.MutableDisplayName,
                        Password = x.Password,
                        Salt = x.Salt,
                        Trueskill = trueskill.FirstOrDefault(y => y.Player.Id == x.Id),
                        WinLoss = winloss.FirstOrDefault(z => z.Player.Id == x.Id)
                    });
                    extendedPlayers = input.Desc ? extendedPlayers.OrderByDescending(x => (x.WinLoss.Win) / ((x.WinLoss.Win + x.WinLoss.Loss))) :
                        extendedPlayers.OrderBy(x => (x.WinLoss.Win) / ((x.WinLoss.Win + x.WinLoss.Loss)));
                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            ReasonPhrase = string.Format("OrderBy value '{0}' is unknown.", input.OrderBy)
                        });
                }

                if (input.IncludeExtendedData && extendedPlayers == null)
                {
                    // we have a queryable to deal with then
                    extendedPlayers = queryAble.ToList().Select(x => new ExtendedPlayer()
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName,
                        MutableDisplayName = x.MutableDisplayName,
                        Password = x.Password,
                        Salt = x.Salt,
                        Trueskill = trueskill.FirstOrDefault(y => y.Player.Id == x.Id),
                        WinLoss = winloss.FirstOrDefault(z => z.Player.Id == x.Id)
                    });
                }

                if (input.StartIndex != 0)
                {
                    if (extendedPlayers == null)
                    {
                        queryAble = queryAble.Skip(input.StartIndex - 1);
                    }
                    else
                    {
                        extendedPlayers = extendedPlayers.Skip(input.StartIndex - 1);
                    }
                }

                if (input.Limit != 0)
                {
                    if (extendedPlayers == null)
                    {
                        queryAble = queryAble.Take(input.Limit);
                    }
                    else
                    {
                        extendedPlayers = extendedPlayers.Take(input.Limit);
                    }
                }

                if (extendedPlayers == null)
                {
                    var playersResult = queryAble.ToList(); // resolve
                    extendedPlayers = playersResult.Select(x => new ExtendedPlayer()
                    {
                        Id = x.Id,
                        DisplayName = x.DisplayName,
                        MutableDisplayName = x.MutableDisplayName,
                        Password = x.Password,
                        Salt = x.Salt
                    });
                }

                extendedPlayers = extendedPlayers.ToList(); // load

                // strip away player data
                foreach (var player in extendedPlayers)
                {
                    if(player.Trueskill != null)
                        player.Trueskill.Player = null;
                    if(player.WinLoss != null)
                        player.WinLoss.Player = null;
                }
                return extendedPlayers;
            }
        }
    }
}
