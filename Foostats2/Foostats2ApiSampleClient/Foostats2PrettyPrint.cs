using System.Collections.Generic;
using System.Linq;
using Foostats2Contracts;
using System;
using System.IO;

namespace Foostats2ApiSampleClient
{
    public static class Foostats2PrettyPrint
    {
        public static void PrettyPrint(this Player player, TextWriter outWriter = null)
        {
            if (outWriter == null)
            {
                outWriter = Console.Out;
            }

            outWriter.WriteLine("{0} ({1}):\n\tDisplay Name: {2}", player.Alias, player.Id, player.MutableDisplayName);
        }

        public static void PrettyPrint(this ExtendedPlayer player, TextWriter outWriter = null)
        {
            if (outWriter == null)
            {
                outWriter = Console.Out;
            }

            outWriter.WriteLine("{0} ({1}):\n\tDisplay Name: {2}\n\tTrueskill: {3}\n\tWin - Loss: {4} - {5}", 
                player.Alias, 
                player.Id, 
                player.MutableDisplayName, 
                player.Trueskill.ConservativeRating,
                player.WinLoss.Win,
                player.WinLoss.Loss);
        }

        public static void PrettyPrint(this IEnumerable<Player> players, TextWriter outWriter = null)
        {
            if (outWriter == null)
            {
                outWriter = Console.Out;
            }

            outWriter.WriteLine("Players: \n\t{0}",
                players.Select(x => string.Format("{0} ({1})", x.Alias, x.MutableDisplayName))
                .Aggregate((x, y) => x + ", " + y));
        }

        public static void PrettyPrint(this IEnumerable<ExtendedPlayer> players, TextWriter outWriter = null)
        {
            if (outWriter == null)
            {
                outWriter = Console.Out;
            }

            if (players.Any())
            {
                if (players.First().Trueskill != null)
                {
                    outWriter.WriteLine("ExtendedPlayers: \n\t{0}",
                        players.Select(x => string.Format("{0} ({1}) {2}", x.Alias, x.MutableDisplayName, x.Trueskill.ConservativeRating))
                        .Aggregate((x, y) => x + ", " + y));
                }
                else
                {
                    outWriter.WriteLine("Players: \n\t{0}",
                        players.Select(x => string.Format("{0} ({1})", x.Alias, x.MutableDisplayName))
                        .Aggregate((x, y) => x + ", " + y));
                }
            }
        }
    }
}
