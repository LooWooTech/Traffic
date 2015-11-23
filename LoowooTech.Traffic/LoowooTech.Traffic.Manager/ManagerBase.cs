using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Manager
{
    public class ManagerBase
    {
        protected TrafficDbContext GetTrafficDataContext()
        {
            var db = new TrafficDbContext();
            db.Database.Connection.Open();
            return db;
        }
    }
}
