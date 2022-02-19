using CoreDBModels;
using System;
using System.ComponentModel;
using System.Data.Entity;

namespace CorePlugin 
{
    public class CoreDBContext : DbContext
    {
        public CoreDBContext() : base(CoreDBModels.DBInfo.ConnectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<CoreDBContext>());
        }

        //字典
        public DbSet<SysDic> SysDic { get; set; }
        //人员
        public DbSet<Staff> Staff { get; set; }
        //系统账户
        public DbSet<User> User { get; set; }
        //日志
        public DbSet<Log> Log { get; set; }
    }

}