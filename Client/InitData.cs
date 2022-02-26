
using CoreDBModels;
using Panuon.UI.Silver;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
                            StaffId = "",
                            CreateTime = DateTime.Now,
                            Creator = 0,
                            DelTime = DateTime.Now,
                            DelUser = 0,
                            IsDel = false
                        });
                        context.SaveChanges();
                    }
                    else
                    {
                        user = context.User.First(c => c.Name == "admin");
                    }

                    #endregion

                    //#region 插件

                    //if (!context.Plugins.Any())
                    //{
                    //    //获取项目根目录
                    //    string basePath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("Client"));
                    //    context.Plugins.Add(new Plugins()
                    //    {
                    //        DBModelUrl = $@"{basePath}CoreDBModels\bin\Debug\CoreDBModels.dll",
                    //        DLLName = "CorePlugin",
                    //        ModuleFolderPaths = "Pages",
                    //        ModuleIcon = "\xf260",
                    //        ModuleTitles = "管理中心",
                    //        Order = 0,
                    //        Title = "核心功能",
                    //        UpdateTime = DateTime.Now,
                    //        UpdateUrl = $@"{basePath}CorePlugin\bin\Debug\CorePlugin.dll"
                    //    });
                    //}

                    //if (!context.RolePlugins.Any())
                    //{
                    //    context.RolePlugins.Add(new RolePlugins()
                    //    {
                    //        RoleId = role.Id,
                    //        UpdateTime = DateTime.Now,
                    //        Pages = ""
                    //    });
                    //}

                    //#endregion

                    //如果没有dic 添加dic数据

                    #region 数据字典

                    if (!context.SysDic.Any())
                    {
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "角色",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "角色",
                            ParentCode = "",
                            QuickCode = DicData.Role
                        });
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "组织架构",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "组织架构",
                            ParentCode = "",
                            QuickCode = DicData.JobPost
                        });
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "支付方式",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "支付方式",
                            ParentCode = "",
                            QuickCode = DicData.PayModel
                        });
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "现金",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "现金",
                            ParentCode = DicData.PayModel,
                            QuickCode = DicData.PayModel + "-" + "现金".Convert2Pinyin()
                        });
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "微信",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "微信",
                            ParentCode = DicData.PayModel,
                            QuickCode = DicData.PayModel + "-" + "微信".Convert2Pinyin()
                        });
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "支付宝",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "支付宝",
                            ParentCode = DicData.PayModel,
                            QuickCode = DicData.PayModel + "-" + "支付宝".Convert2Pinyin()
                        });
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "供应商类型",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "供应商类型",
                            ParentCode = "",
                            QuickCode = DicData.SupplierType
                        });
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "物品类型",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "物品类型",
                            ParentCode = "",
                            QuickCode = DicData.GoodsType
                        });
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "物品单位",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "物品单位",
                            ParentCode = "",
                            QuickCode = DicData.GoodsUnit
                        });
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "仓库",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "仓库",
                            ParentCode = "",
                            QuickCode = DicData.Store
                        });
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "固定资产状态",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "固定资产状态",
                            ParentCode = "",
                            QuickCode = DicData.FixedAssetsState
                        });
                        context.SysDic.Add(new SysDic()
                        {
                            Content = "固定资产位置",
                            Creater = 0,
                            CreateTime = DateTime.Now,
                            Name = "固定资产位置",
                            ParentCode = "",
                            QuickCode = DicData.FixedAssetsLocation
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
