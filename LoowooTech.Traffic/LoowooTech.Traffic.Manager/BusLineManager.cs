using LoowooTech.Traffic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Manager
{
    public class BusLineManager:ManagerBase
    {
        public void Add(BusLine busLine)
        {
            using (var db = GetTrafficDataContext())
            {
                db.BusLines.Add(busLine);
                db.SaveChanges();
            }
        }

        public void Update(BusLine busLine)
        {
            using (var db = GetTrafficDataContext())
            {
                var entry = db.BusLines.Find(busLine.ID);
                if (entry == null)
                {
                    db.BusLines.Add(busLine);
                }
                else
                {
                    busLine.ID = entry.ID;
                    db.Entry(entry).CurrentValues.SetValues(busLine);    
                }
                db.SaveChanges();
            }
        }

        public void Add(List<BusLine> List)
        {
            foreach (var item in List) 
            {
                Update(item);
                //Add(item); 
            }
        }

        public List<BusLine> Get(string Number)
        {
            using (var db = GetTrafficDataContext())
            {
                return db.BusLines.Where(e => e.ShortName == Number).ToList();
            }
        }
    }
}
