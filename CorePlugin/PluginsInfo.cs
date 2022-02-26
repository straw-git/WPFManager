using Common;
using System.Collections.Generic;

namespace CorePlugin
{
    public class PluginsInfo : BasePlugins
    {
        public readonly string pluginsTitle = "核心功能";
        public readonly string dllName = "CorePlugin";
        public readonly string logoImageName = "logo.jpg";

        public PluginsInfo()
        {
            PluginsTitle = pluginsTitle;
            PluginsDLLName = dllName;
            Order = 0;

            #region 注入导航

            string basePath = $"pack://application:,,,/{dllName};component/";

            //管理中心
            ModuleInfo managerModule = new ModuleInfo()
            {
                FullPath = $"{dllName}/Pages",
                Title = "管理中心",
                Icon = "\xf0f0",
                Order = 0
            };
            List<PageInfo> managerPages = new List<PageInfo>()
            {
                new PageInfo(){ Title="首页",FullPath=$"{basePath}Pages/Manager/Index.xaml",Order=0,Icon="\xf015" },
                new PageInfo(){ Title="系统账号",FullPath=$"{basePath}Pages/Manager/User.xaml",Order=1,Icon="\xf2bc" },
                new PageInfo(){ Title="数据字典",FullPath=$"{basePath}Pages/Manager/Dic.xaml",Order=2 ,Icon="\xf1ac"},
                new PageInfo(){ Title="插件管理",FullPath=$"{basePath}Pages/Manager/PluginsMsg.xaml",Order=3,Icon="\xf260" }
            };

            RegisterPages(managerModule, managerPages);

            #endregion 
        }
    }
}
