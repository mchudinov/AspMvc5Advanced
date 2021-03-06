﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Models;

namespace Repositories
{
    [Log]
    [LogException]
    public class UserRepository : IUserRepository
    {
        public IList<User> GetUsers()
        {
            using (var db = new AppDbContext())
            {
                return db.Users.AsNoTracking().OrderBy(u=>u.Nickname).ToList();
            }
        }

        public User GetUser(Guid id)
        {
            using (var db = new AppDbContext())
            {
                return db.Users.FirstOrDefault(u => u.Id == id);
            }
        }

        public IList<User> GetUsers(string searchString)
        {
            using (var db = new AppDbContext())
            {
                return db.Users.AsNoTracking().Where(u => u.Nickname.ToLower().Contains(searchString.ToLower())).OrderBy(u => u.Nickname).ToList();
            }
        }

        public async Task SaveUser(User user)
        {
            using (var db = new AppDbContext())
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteUser(Guid id)
        {
            using (var db = new AppDbContext())
            {
                var user = GetUser(id);
                db.Users.Attach(user);
                db.Users.Remove(user);
                await db.SaveChangesAsync();
            }
        }
    }
}
