using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Foostats2.Controllers.API;
using Foostats2.Models;
using System.Data.Entity;

namespace Foostats2.Controllers.API
{
    public class MatchApiController : FoostatsApiController
    {
        /// <summary>
        /// Do not go over this many ListAll results, prevents long requests 
        /// from happening. If you need more than 1000 results use List
        /// with StartIndex and Limit.
        /// </summary>
        private const int MaxListAll = 1000;

        [HttpGet]
        public IEnumerable<Match> ListAll()
        {
            using (var db = new FoostatsContext())
            {
                return db.Matches
                              .Include(x => x.Team1)
                              .Include(x => x.Team2)
                              .Include(x => x.Team1.Player1)
                              .Include(x => x.Team1.Player2)
                              .Include(x => x.Team2.Player1)
                              .Include(x => x.Team2.Player2)
                              .Take(MaxListAll)
                              .ToList();
            }
        }

        [HttpPost]
        [ActionName("List")]
        public IEnumerable<Match> ListAllOrderedBy(ListAllInput input)
        {
            using (var db = new FoostatsContext())
            {
                var dbSet = db.Matches
                              .Include(x => x.Team1)
                              .Include(x => x.Team2)
                              .Include(x => x.Team1.Player1)
                              .Include(x => x.Team1.Player2)
                              .Include(x => x.Team2.Player1)
                              .Include(x => x.Team2.Player2);
                if (!string.IsNullOrEmpty(input.SearchKey))
                {
                    if (string.Compare(input.SearchKey, "Alias", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        if (!string.IsNullOrEmpty(input.SearchTerm))
                        {
                            dbSet = db.Matches.Where(x => (x.Team1.Player1.DisplayName == input.SearchTerm ||
                                                   x.Team1.Player2.DisplayName == input.SearchTerm ||
                                                   x.Team2.Player1.DisplayName == input.SearchTerm ||
                                                   x.Team2.Player2.DisplayName == input.SearchTerm));
                        }
                        else
                        {
                            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                                {
                                    ReasonPhrase =
                                        string.Format("SearchTerm value unspecified for SearchKey {0}.", input.SearchKey)
                                });
                        }
                    }
                    else
                    {
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            ReasonPhrase =
                                string.Format("SearchKey value '{0}' is unknown.", input.SearchKey)
                        });
                    }
                }

                IQueryable<Match> queryAble = null;
                if (string.IsNullOrEmpty(input.OrderBy))
                {
                    input.OrderBy = "Date"; // date by default
                }

                if (string.Compare(input.OrderBy, "Date", StringComparison.InvariantCulture) == 0)
                {
                    queryAble = input.Desc
                                    ? dbSet.OrderByDescending(x => x.Team1Validated)
                                    : dbSet.OrderBy(x => x.Team1Validated);
                }
                else
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        ReasonPhrase = string.Format("OrderBy value '{0}' is unknown.", input.OrderBy)
                    });
                }

                if (input.StartIndex != 0)
                {
                    queryAble = queryAble.Skip(input.StartIndex - 1);
                }

                if (input.Limit > 0)
                {
                    queryAble = queryAble.Take(input.Limit);
                }

                return queryAble.ToList();
            }
        }

        [HttpPost]
        [ActionName("AddMatch")]
        public HttpResponseMessage AddMatch(MatchInput input)
        {
            AddMatchInternal(
                input.Team1Player1Alias, 
                input.Team1Player2Alias, 
                input.Team2Player1Alias, 
                input.Team2Player2Alias, 
                input.Team1Score, 
                input.Team2Score);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
