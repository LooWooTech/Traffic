using LoowooTech.Traffic.Models;
using LoowooTech.Traffic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

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
        public User Login(string Name, string Password)
        {
            if (string.IsNullOrEmpty(Password)||string.IsNullOrEmpty(Name))
            {
                return null;
            }
            return  Search(Name, Password);
        }
        public User Search(int ID)
        {
            using (var db = GetTrafficDataContext())
            {
                return db.Users.Find(ID);
            }
        }

        public User Search(string Name, string Password)
        {
            using (var db = GetTrafficDataContext())
            {
                Password = Password.MD5();
                return db.Users.Where(e => e.Name == Name && e.Password == Password).FirstOrDefault();
            }
        }
        public bool Delete(int ID)
        {
            using (var db = GetTrafficDataContext())
            {
                var user = db.Users.Find(ID);
                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        public bool Edit(int ID, Role role)
        {
            using (var db = GetTrafficDataContext())
            {
                var user = db.Users.Find(ID);
                if (user != null)
                {
                    user.Role = role;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public void Edit(User user)
        {
            using (var db = GetTrafficDataContext())
            {
                var entry = db.Users.Find(user.ID);
                if (entry != null)
                {
                    db.Entry(entry).CurrentValues.SetValues(user);
                    db.SaveChanges();
                }
            }
        }

        public List<User> GetList()
        {
            using (var db = GetTrafficDataContext())
            {
                return db.Users.ToList();
            }
        }
    }
}
