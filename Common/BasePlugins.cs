using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Common
{
    /// <summary>
    /// 插件中的模块
    /// </summary>
    public class BasePlugins
    {
        /// <summary>
        /// 插件的数据库Id
        /// </summary>
        public int Id = 0;
        /// <summary>
        /// 插件标题
        /// </summary>
        public string PluginsTitle = "";
        /// <summary>
        /// 插件DLL名称
        /// </summary>
        public string PluginsDLLName = "";
        /// <summary>
        /// 插件中的模块
        /// Key 模块名称
        /// Value 文件夹路径
        /// </summary>
        public List<ModuleInfo> Modules = new List<ModuleInfo>();
        /// <summary>
        /// 规定插件显示的Logo图 默认[根目录（不带 /）logo.jpg]
        /// </summary>
        public string logo = "logo.jpg";
        /// <summary>
        /// 插件排序
        /// </summary>
        public int Order = 0;

        /// <summary>
        /// 模块中的页面
        /// Key 模块名称
        /// value 页面路径
        /// value.key 页面标题
        /// value.value 页面路径
        /// </summary>
        public Dictionary<string, List<PageInfo>> Pages = new Dictionary<string, List<PageInfo>>();

        /// <summary>
        /// 注册模块
        /// </summary>
        /// <param name="_moduleTitle">标题</param>
        /// <param name="_modulePath">地址</param>
        /// <param name="_moduleIcon">icon</param>
        /// <param name="_pages">页面集合</param>
        public void RegisterPages(ModuleInfo _module, List<PageInfo> _pages)
        {
            if (Modules.Any(c => c.FullPath == _module.FullPath))
            {
                //存在模块 说明多次添加 抛出异常
                throw new Exception();
            }
            else
            {
                if (Modules.Any(c => c.Title == _module.Title)) 
                {
                    //存在相同标题 抛出异常
                    throw new Exception();
                }
                //模块不存在
                Modules.Add(_module);
                Pages.Add(_module.Title, _pages);
            }
        }
    }
}
