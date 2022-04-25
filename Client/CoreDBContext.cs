
using CoreDBModels;
using System.Data.Entity;

namespace Client
{
    public class CoreDBContext : DbContext
    {
        public CoreDBContext() : base(DBConnections.Get("CoreConnectionStr"))
        {
            //首次运行时 不存在数据库 进行初始化
            Database.SetInitializer(new CreateDatabaseIfNotExists<CoreDBContext>());
        }

        #region 基础

        public DbSet<User> User { get; set; }//系统账户
        public DbSet<CoreSetting> CoreSetting { get; set; }//设置
        public DbSet<Role> Role { get; set; }//系统账户角色

        #endregion

        #region  权限相关

        public DbSet<Plugins> Plugins { get; set; }//插件
        public DbSet<PluginsModule> PluginsModule { get; set; }//插件中的模型
        public DbSet<ModulePage> ModulePage { get; set; }//模型中的页面
        public DbSet<RolePlugins> RolePlugins { get; set; }//角色的插件
        public DbSet<UserPlugins> UserPlugins { get; set; }//用户的插件

        #endregion

        #region 消息 通知

        public DbSet<Email> Email { get; set; }//邮件
        public DbSet<Notice> Notice { get; set; }//通知

        #endregion 
    }
}
