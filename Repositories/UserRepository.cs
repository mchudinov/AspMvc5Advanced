﻿using System;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        public IList<User> GetAllUsers()
        {
            using (var db = new AppDbContext())
            {
                return db.Users.ToList();
            }
        }

        public User GetUser(Guid guid)
        {
            using (var db = new AppDbContext())
            {
                return db.Users.First(u => u.Id == guid);
            }
        }

        public IList<User> GetUsers(string filter)
        {
            using (var db = new AppDbContext())
            {
                return db.Users.Where(u => u.Nickname.ToLower().Contains(filter.ToLower())).ToList();
            }
        }

        public void SaveUser(User user)
        {
            using (var db = new AppDbContext())
            {
                db.Users.Add(user);
                db.SaveChangesAsync();
            }
        }
    }
}
