using Client.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.UserGlobal;

namespace Client.CurrGlobal
{
    public class MainWindowsGlobal
    {
        /// <summary>
        /// 所有主窗体
        /// </summary>
        public static Dictionary<string, MainWindow> MainWindowsDic = new Dictionary<string, MainWindow>();

        /// <summary>
        /// 将数据加入到窗体中
        /// </summary>
        /// <param name="_windowName">窗体名、标题</param>
        /// <param name="_models">插件数据</param>
        public static void Data2MainWindow(string _windowName, List<PluginsModel> _models)
        {
            MainWindow mainWindow;
            if (MainWindowsDic.ContainsKey(_windowName))
            {
                mainWindow = MainWindowsDic[_windowName];
            }
            else
            {
                mainWindow = new MainWindow(_windowName);
                MainWindowsDic.Add(_windowName, mainWindow);//保存窗体
            }
            mainWindow.AddPluginModels(_models);
            mainWindow.Show();
        }

        /// <summary>
        /// 获取所有窗体名称
        /// </summary>
        /// <returns></returns>
        public static List<string> GetWindowNames()
        {
            return MainWindowsDic.Keys.ToList();
        }
    }
}
