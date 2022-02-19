
using CoreDBModels;
using System;
using System.ComponentModel;
using System.Data.Entity;

namespace ERPPlugin 
{
    public class CoreDBContext : DbContext
    {
        public CoreDBContext() : base(CoreDBModels.DBInfo.ConnectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<CoreDBContext>());
        }

        public DbSet<SysDic> SysDic { get; set; }
        public DbSet<User> User { get; set; }

    }

}