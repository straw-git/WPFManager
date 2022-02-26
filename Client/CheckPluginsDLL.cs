using Common;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static Common.UserGlobal;

namespace Client
{
    /// <summary>
    /// 检查插件dll
    /// </summary>
    public class CheckPluginsDLL
    {
        /// <summary>
        /// 插件介绍类名
        /// </summary>
        public static string PluginsClassName = "PluginsInfo";
        /// <summary>
        /// 插件文件夹名称
        /// </summary>
        public static string PluginFolderName = "plugins";

        /// <summary>
        /// 查找dll中的Page所在的空间
        /// </summary>
        /// <param name="_dllName"></param>
        public static BasePlugins GetPluginsModel(string _dllName, int _pluginId)
        {
            //判断dll文件是否存在
            if (!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}plugins\\{_dllName}.dll"))
            {
                Notice.Show($"未发现[ /{PluginFolderName}/{_dllName}.dll ] 文件", $"{_dllName}加载失败");
                return null;
            }
            //获取PluginsInfo
            BasePlugins pluginsInfo = null;
            try
            {
                Assembly currAssembly = Assembly.LoadFrom($"{PluginFolderName}\\{_dllName}.dll");
                pluginsInfo = (BasePlugins)Activator.CreateInstance(currAssembly.GetType($"{_dllName}.{PluginsClassName}"));
            }
            catch
            {
                return null;
            }

            return pluginsInfo;
        }
    }
}
