using Common;

namespace CorePlugin
{
    public class PluginsInfo : BasePlugins
    {
        public readonly string pluginsCode = "核心功能";
        public readonly string[] pages = { "Pages" };
        public readonly string dllName = "CorePlugin";
        public readonly string menuClssName = "MenuInfo";//规定插件中页面目录的文件名称 继承与BaseMenuInfo
        public readonly string logoImageName = "logo.jpg";//规定插件显示的Logo图 默认 [ 根目录（不带/）logo.jpg ] 

        public PluginsInfo()
        {
            MenuClassName = menuClssName;
            LogoImageName = logoImageName;
            Name = pluginsCode;//添加插件编码
            DLLName = dllName;
            foreach (var p in pages)
            {
                AddPageFolderName(p);//添加Pages为页面文件夹
            }
        }
    }
}
