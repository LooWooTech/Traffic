using LoowooTech.Traffic.Models;
using System.Data.Entity;

namespace LoowooTech.Traffic.Manager
{
    public class TrafficDbContext : DbContext
    {
        public TrafficDbContext() : base("name=TRAFFIC") { }
        public TrafficDbContext(string connectionString) : base(connectionString) { }
        public DbSet<BusLine> BusLines { get; set; }
    }
}
