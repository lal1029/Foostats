using System.Data.Entity;

namespace Foostats2.Models
{
    public class FoostatsContext : DbContext
    {
        public DbSet<Match> Matches { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Trueskill> Trueskill { get; set; }
        public DbSet<RegistryEntry> Registry { get; set; }
        public DbSet<WinLoss> WinLoss { get; set; }
    }
}
