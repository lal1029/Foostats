using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Foostats2.Filters;
using Foostats2.Metrics;
using Foostats2.Metrics.Plugins;
using Foostats2.Models;

namespace Foostats2.Controllers
{
    [PopulateFoostatsViewContext]
    public class FoostatsController : Controller
    {
        protected List<IOnlineMetricCalculator> MetricCalculators; 

        public FoostatsController()
        {
            // intialize the metric calculators
            var metricCalculatorsList = new List<IOnlineMetricCalculator>();
            metricCalculatorsList.Add(new TrueskillCalculator());
            metricCalculatorsList.Add(new WinLossCalculator());
            MetricCalculators = metricCalculatorsList;
        }

        protected IQueryable<Match> GetMatches(FoostatsContext context, int playerId)
        {
            return context.Matches.Include(x => x.Team1).Include(x => x.Team2).
                    Include(x => x.Team1.Player1).
                    Include(x => x.Team1.Player2).
                    Include(x => x.Team2.Player1).
                    Include(x => x.Team2.Player2).Where(x =>
                        x.Team1.Player1.Id == playerId ||
                        x.Team1.Player2.Id == playerId ||
                        x.Team2.Player1.Id == playerId ||
                        x.Team2.Player2.Id == playerId).OrderByDescending(x => x.Team1Validated);
        }
    }
}
