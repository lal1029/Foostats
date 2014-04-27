using System;
using System.Linq;
using Foostats2Contracts;

namespace Foostats2ApiSampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            const string foostatsEndpoint = "http://localhost:25027/Api/";
            var foostats2ApiClient = new Foostats2ApiClient(new Uri(foostatsEndpoint));
            
            /*
             * List All Players
             */

            /*
             * Async/Await Task Style
             */
            Console.WriteLine("=======");
            Console.WriteLine("List all players async:");
            var playerTask = foostats2ApiClient.ListAllPlayersAsync();
            playerTask.Wait();
            var players = playerTask.Result;
            players.PrettyPrint();

            /*
             * Synchronous
             */
            Console.WriteLine("=======");
            Console.WriteLine("List all players:");
            players = foostats2ApiClient.ListAllPlayers();
            players.PrettyPrint();

            /*
             * Get Player Details for a player
             */
            if(players.Any())
            {
                Console.WriteLine("=======");
                Console.WriteLine("Get player details for alias:");
                var player = foostats2ApiClient.GetPlayerByAlias(players.First().Alias);
                player.PrettyPrint();
            }
            
            /*
             * Get Top 5 Players by Trueskill
             */
            Console.WriteLine("=======");
            Console.WriteLine("Top 5 players by Trueskill:");
            var extendedPlayers = foostats2ApiClient.ListPlayers(new ListAllInput()
                {
                    Desc = true,
                    IncludeExtendedData = true,
                    Limit = 10,
                    OrderBy = "Trueskill"
                });
            extendedPlayers.PrettyPrint();

            /*
             * Add a Match
             * NOTE: DO NOT ADD MATCHES AGAINST PROD!!!
             */
            if (!foostatsEndpoint.Contains("foostats"))
            {
                Console.WriteLine("=======");
                Console.WriteLine("Adding a new match:");
                foostats2ApiClient.AddMatch(new MatchInput()
                {
                    Team1Player1Alias = "REDMOND\\amayoub",
                    Team2Player1Alias = "REDMOND\\anhiggins",
                    Team1Score = 8,
                    Team2Score = 7
                });
            }
            

            /*
             * Get Top 5 Players by Trueskill Again
             */
            Console.WriteLine("=======");
            Console.WriteLine("Top 5 players by Trueskill:");
            extendedPlayers = foostats2ApiClient.ListPlayers(new ListAllInput()
            {
                Desc = true,
                IncludeExtendedData = true,
                Limit = 5,
                OrderBy = "Trueskill"
            });
            extendedPlayers.PrettyPrint();

            // Pause
            Console.WriteLine("=======");
            Console.WriteLine("DONE. (press any key)");
            Console.Read();
        }
    }
}
