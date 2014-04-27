using Foostats2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foostats2.Metrics
{
    /// <summary>
    /// Interface that defines an online metric calculator, online in this context meaning
    /// one that can take in value continuously and update the metrics (does not a complete
    /// set of data to calculate the appropriate values)
    /// </summary>
    public interface IOnlineMetricCalculator
    {
        void UpdateMetrics(Team Team1, Team Team2, int Score1, int Score2);
    }
}