
using Client.Helper;
using Common.Data.Local;
using DBModels.Sys;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{

    public class SoftInfo 
    {
        /// <summary>
        /// 程序开始运行时间
        /// </summary>
        public static DateTime SoftStartTime;
    }

    public class GlobalEvent
    {
        public static void OnClientStart()
        {
            //初始化插件
            LocalPlugins.Init();
            //加载所有皮肤
            LocalSkin.Init();
            //加载用户设置
            LocalSettings.Init();
            //初始化数据库连接数据
            LocalDB.Init();
            //服务端设置
            LocalServer.Init();
            //初始化样式
            StyleHelper.Init();
        }
    }

    public class UserGlobal
    {
        //是否已登录
        public static bool IsLogin = false;
        //当前用户信息
        public static User CurrUser;
    }

    public class UIGlobal
    {
        /// <summary>
        /// 运行UI线程
        /// </summary>
        /// <param name="action"></param>
        public static void RunUIAction(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }

    public class WindowGlobal
    {
        public static MainWindow MainWindow;

        public static void EnableMainWindow(bool _enable)
        {
            MainWindow.IsEnabled = _enable;
        }

        public static void MaskVisible(bool _visible)
        {
            MainWindow.IsMaskVisible = _visible;
        }
    }
}
