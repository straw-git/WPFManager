using CoreDBModels;
using System;
using System.ComponentModel;
using System.Data.Entity;

namespace CorePlugin
{
    public class CoreDBContext : DbContext
    {
        public CoreDBContext() : base(DBConnections.Get("CoreConnectionStr"))
        {
            //首次运行时 不存在数据库 进行初始化
            Database.SetInitializer(new CreateDatabaseIfNotExists<CoreDBContext>());
        }

        public DbSet<User> User { get; set; }//系统账户
        public DbSet<Log> Log { get; set; }//日志
        public DbSet<CoreSetting> CoreSetting { get; set; }//设置
        public DbSet<Role> Role { get; set; }//系统账户角色

        #region  权限相关

        public DbSet<Plugins> Plugins { get; set; }
        public DbSet<PluginsModule> PluginsModule { get; set; }
        public DbSet<ModulePage> ModulePage { get; set; }
        public DbSet<RolePlugins> RolePlugins { get; set; }
        public DbSet<UserPlugins> UserPlugins { get; set; }

        #endregion
    }

}