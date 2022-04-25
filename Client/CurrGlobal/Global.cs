
using Common.Data.Local;
using Common.Utils;
using Hangfire;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client.CurrGlobal
{
    /// <summary>
    /// 全局数据
    /// </summary>
    public partial class Global
    {
        /// <summary>
        /// 程序开始运行时间
        /// </summary>
        public static DateTime SoftStartTime;

        /// <summary>
        /// 没有阅读的邮件数
        /// </summary>
        public static int NotReadEmailCount { get; set; }

        /// <summary>
        /// 没有阅读的通知数
        /// </summary>
        public static int NotReadNoticeCount { get; set; }
    }

    /// <summary>
    /// 事件全局
    /// </summary>
    public partial class GlobalEvent
    {

        /// <summary>
        /// 客户端开始
        /// </summary>
        public static void OnClientStart()
        {
            //加载所有皮肤
            LocalSkin.Init();
            //加载用户设置
            LocalSettings.Init();
            //初始化样式
            StyleHelper.Init();
        }

        /// <summary>
        /// 开始定期读取信息
        /// </summary>
        public static void StartReadMessage()
        {
        //    RecurringJob.AddOrUpdate(() => OnReadMessageDaily(),TimeSpan.FromMinutes(10));
        //https://github.com/HangfireIO/Hangfire
        }

    }

    /// <summary>
    /// 页面全局
    /// </summary>
    public partial class UIGlobal
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
