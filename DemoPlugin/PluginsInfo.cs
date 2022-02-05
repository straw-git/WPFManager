using Common;

namespace DemoPlugin
{
    public class PluginsInfo : BasePlugins
    {
        public static readonly string pluginsCode = "Demo";
        public static readonly string[] pages = { "Pages" };

        static PluginsInfo()
        {
            Code = pluginsCode;//添加插件编码
            foreach (var p in pages)
            {
                AddPageFolderName(p);//添加Pages为页面文件夹
            }
        }
    }
}
