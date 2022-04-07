﻿
using Common.Data.Local;
using Common.Utils;
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
            //加载所有皮肤
            LocalSkin.Init();
            //加载用户设置
            LocalSettings.Init();
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
