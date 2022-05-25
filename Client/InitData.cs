
using CoreDBModels;
using Panuon.UI.Silver;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Client
{
    public class InitData
    {
        /// <summary>
        /// 填充管理员数据
        /// </summary>
        public static bool NullDataCheck()
        {
            try
            {
                User user = null;
                Role role = null;
                using (var context = new CoreDBContext())
                {
                    //如果没有任何信息 则添加默认管理员账户 admin/123456
                    #region 角色

                    if (!context.Role.Any())
                    {
                        role = context.Role.Add(new Role()
                        {
                            DelTime = DateTime.Now,
                            DelUser = 0,
                            DelUserName = "",
                            IsDel = false,
                            Name = "超级管理员"
                        });
                        context.SaveChanges();
                    }

                    #endregion 

                    #region admin 账号

                    if (!context.User.Any())
                    {
                        user = context.User.Add(new User()
                        {
                            Name = "admin",
                            Pwd = "123456",
                            CanLogin = true,
                            RoleId = role.Id,
                            CreateTime = DateTime.Now,
                            Creator = 0,
                            DelTime = DateTime.Now,
                            DelUser = 0,
                            IsDel = false,
                            DeparmentId = 0,
                            DepartmentPositionId = 0,
                            IdCard = "",
                            NewPositionId = 0,
                            NewPositionStartTime = DateTime.Now,
                            PositionEndTime = DateTime.Now,
                            PositionType = 0,
                            RealName="超级管理员"
                        }) ;
                        context.SaveChanges();
                    }
                    else
                    {
                        user = context.User.First(c => c.Name == "admin");
                    }

                    #endregion

                    #region 基础设置

                    if (!context.CoreSetting.Any())
                    {
                        context.CoreSetting.Add(new CoreSetting()
                        {
                            MaxLogCount = 500,
                            PluginsUpdateBaseUrl = "http://127.0.0.1:8088/"
                        });
                    }

                    #endregion 

                    #region 基础插件

                    if (!context.Plugins.Any())
                    {
                        Plugins plugins = context.Plugins.Add(new Plugins()
                        {
                            DLLName = "CorePlugin",
                            LogoImage = "logo.jpg",
                            Name = "基础管理",
                            Order = 0,
                            UpdateTime = DateTime.Now,
                            WebDownload = true,
                            ConnectionName = "CoreConnectionStr",
                            ConnectionString = @"Data Source=PC-20201105CMZQ\SQLEXPRESS;Initial Catalog=ZDB;User ID=sa;Password=123456;"
                        });
                        context.SaveChanges();
                        PluginsModule pluginsModule = context.PluginsModule.Add(new PluginsModule()
                        {
                            ModuleName = "管理中心",
                            PluginsId = plugins.Id,
                            Icon = "fa-user-md",
                            Order = 1
                        });
                        context.SaveChanges();
                        context.ModulePage.Add(new ModulePage()
                        {
                            ModuleId = pluginsModule.Id,
                            Order = 0,
                            PageName = "首页",
                            PagePath = "Pages/Manager/Index.xaml",
                            PluginsId = plugins.Id,
                            Icon = "fa-home"
                        });
                        context.ModulePage.Add(new ModulePage()
                        {
                            ModuleId = pluginsModule.Id,
                            Order = 0,
                            PageName = "人员管理",
                            PagePath = "Pages/Manager/User.xaml",
                            PluginsId = plugins.Id,
                            Icon = "fa-address-card-o"
                        });
                        context.ModulePage.Add(new ModulePage()
                        {
                            ModuleId = pluginsModule.Id,
                            Order = 0,
                            PageName = "角色授权",
                            PagePath = "Pages/Manager/RoleAuthorization.xaml",
                            PluginsId = plugins.Id,
                            Icon = "fa-key"
                        });
                        context.ModulePage.Add(new ModulePage()
                        {
                            ModuleId = pluginsModule.Id,
                            Order = 0,
                            PageName = "插件管理",
                            PagePath = "Pages/Manager/PluginsMsg.xaml",
                            PluginsId = plugins.Id,
                            Icon = "fa-paperclip"
                        });
                        context.ModulePage.Add(new ModulePage()
                        {
                            ModuleId = pluginsModule.Id,
                            Order = 0,
                            PageName = "职位管理",
                            PagePath = "Pages/Manager/DepartmentPositionMsg.xaml",
                            PluginsId = plugins.Id,
                            Icon = "fa-address-card-o"
                        });
                    }

                    #endregion 

                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                UIGlobal.RunUIAction(() =>
                {
                    MessageBoxX.Show(ex.Message, "数据连接错误");
                });
                return false;
            }
        }
    }
}
