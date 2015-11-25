using LoowooTech.Traffic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Manager
{
    public class UserManager:ManagerBase
    {
        public void Add(User user)
        {
            using (var db = GetTrafficDataContext())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public User Search(string Name, string Password)
        {
            using (var db = GetTrafficDataContext())
            {
                return db.Users.Where(e => e.Name == Name && e.Password == Password).FirstOrDefault();
            }
        }
    }
}
