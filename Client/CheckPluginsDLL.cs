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
        /// 主程序窗口
        /// </summary>
        public static string EXEName = "Client";
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
        /// <param name="_dllOrder">dll排序</param>
        public static PluginsModel GetPluginsModel(string _dllName, int _dllOrder)
        {
            //判断dll文件是否存在
            if (_dllName != EXEName && !File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}plugins\\{_dllName}.dll"))
            {
                Notice.Show("DLL格式不正确,请将页面放置在[ dll命名空间.页面命名空间.导航命名空间 ]下", $"{_dllName}加载失败");
                return null;
            }

            Assembly currAssembly = _dllName == EXEName ? Assembly.GetExecutingAssembly() : Assembly.LoadFrom($"{PluginFolderName}\\{_dllName}.dll");

            //获取PluginsInfo
            BasePlugins pluginsInfo = _dllName == EXEName
                        ? (BasePlugins)Activator.CreateInstance(Type.GetType($"{_dllName}.{PluginsClassName}"))
                        : (BasePlugins)Activator.CreateInstance(currAssembly.GetType($"{_dllName}.{PluginsClassName}"));

            if (pluginsInfo == null) return null;


            PluginsModel pluginsModel = new PluginsModel();
            pluginsModel.Code = pluginsInfo.DLLName;
            pluginsModel.Name = pluginsInfo.Name;
            pluginsModel.Order = _dllOrder;
            pluginsModel.LogoImageSource = new BitmapImage(new Uri($"pack://application:,,,/{pluginsInfo.DLLName};component/{pluginsInfo.LogoImageName}"));
            pluginsModel.Modules = new List<ModuleModel>();

            //根据插件说明中的命名空间查找页面
            string[] pfs = pluginsInfo.PageFolderNames.Split(',');
            foreach (string pf in pfs)
            {
                if (string.IsNullOrEmpty(pf)) continue;

                //查找到的页面
                string _pfNames = $"{_dllName}.{pf}";

                //此处挑选出带有MenuClassName的命名空间（文件夹）
                var _moduleFolders = (from t in currAssembly.GetTypes()
                                      where t.IsClass
                                      && t.Namespace != null
                                      && t.Namespace.StartsWith(_pfNames)
                                      && t.FullName.Contains(pluginsInfo.MenuClassName)
                                      && !t.FullName.Contains("<")
                                      && !t.FullName.Contains(">")
                                      && !t.FullName.Contains("+")
                                      select t.FullName).ToList();

                foreach (var _moduleFolder in _moduleFolders)
                {
                    BaseMenuInfo menuInfo = null;
                    //获取MenuIfo
                    menuInfo = _dllName == EXEName
                        ? (BaseMenuInfo)Activator.CreateInstance(Type.GetType(_moduleFolder))
                                    : (BaseMenuInfo)Activator.CreateInstance(currAssembly.GetType(_moduleFolder));

                    if (menuInfo == null) continue;

                    ModuleModel moduleModel = new ModuleModel();
                    moduleModel.Code = menuInfo.Code;
                    moduleModel.Name = menuInfo.Name;
                    moduleModel.Order = menuInfo.SelfOrder;
                    moduleModel.Pages = new List<PageModel>();

                    //获取命名空间
                    string _moduleFolderNsp = _moduleFolder.Substring(0, _moduleFolder.LastIndexOf('.'));
                    //查找模块下的所有页面
                    var _currPages = (from t in currAssembly.GetTypes()
                                      where t.IsClass
                                      && t.Namespace != null
                                      && t.Namespace.StartsWith(_moduleFolderNsp)
                                      && !t.FullName.Contains(pluginsInfo.MenuClassName)
                                      && !t.FullName.Contains("<")
                                      && !t.FullName.Contains(">")
                                      && !t.FullName.Contains("+")
                                      select t.FullName).ToList();

                    try
                    {
                        foreach (var _currPage in _currPages)
                        {
                            #region 获取页面说明

                            Type itemObj = _dllName == EXEName ? Type.GetType(_currPage) : currAssembly.GetType(_currPage);
                            BasePage itemPage = (BasePage)Activator.CreateInstance(itemObj);
                            if (!itemPage.IsMenu) continue;//不是导航 排除

                            itemPage.Code = $"{_currPage.Substring(_currPage.LastIndexOf('.') + 1)}";

                            #endregion

                            PageModel pageModel = new PageModel();
                            pageModel.Code = itemPage.Title;
                            pageModel.Order = itemPage.Order;
                            pageModel.Url = _dllName == EXEName
                                        ? $"/{pf}/{moduleModel.Code}/{itemPage.Code}.xaml"
                                        : $"pack://application:,,,/{_dllName};component/{pf}/{moduleModel.Code}/{itemPage.Code}.xaml";

                            moduleModel.Pages.Add(pageModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    pluginsModel.Modules.Add(moduleModel);
                }
            }

            return pluginsModel;
        }
    }
}
