
using CoreDBModels;
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// 主窗体公用
    /// </summary>
    public class MainWindowGlobal
    {
        public static BaseMainWindow MainWindow=null;
        /// <summary>
        /// 当前窗体中的插件
        /// </summary>
        public static List<Plugins> CurrPlugins = new List<Plugins>();
        /// <summary>
        /// 当前窗体中的模块
        /// </summary>
        public static List<PluginsModule> CurrPluginsModules = new List<PluginsModule>();
        /// <summary>
        /// 当前窗体中的页面
        /// </summary>
        public static List<ModulePage> CurrModulePages = new List<ModulePage>();


    }
}
