﻿using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Models;

namespace Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=AppDb") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<Widget> Widgets { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
