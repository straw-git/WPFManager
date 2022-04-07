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
        //系统账户
        public DbSet<User> User { get; set; }
        //日志
        public DbSet<Log> Log { get; set; }
        //系统账户角色
        public DbSet<Role> Role { get; set; }
        //权限相关
        public DbSet<Plugins> Plugins { get; set; }
        public DbSet<PluginsModule> pluginsModule { get; set; }
        public DbSet<PluginsDB> PluginsDB { get; set; }
        public DbSet<ModulePage> PluginsPage { get; set; }
        public DbSet<RolePlugins> RolePlugins { get; set; }
        public DbSet<UserPlugins> UserPlugins { get; set; }

        public DbSet<CoreSetting> CoreSetting { get; set; }
    }

}