using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foostats2.Utility
{
    public class FoosHelpers
    {
        public static String getPlayerName(String TeamName)
        {
            var startPosition = TeamName.IndexOf('<') + 1;
            if (startPosition != 0) // implies actual returned value is -1
            {
                var length = TeamName.LastIndexOf('>') - startPosition;
                TeamName = TeamName.Substring(startPosition, length);
            }

            return TeamName;
        }
    }
}