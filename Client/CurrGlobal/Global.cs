
using Client.Helper;
using Common.Data.Local;
using Common.Utils;
using DBModels.Sys;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client.CurrGlobal
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

}
