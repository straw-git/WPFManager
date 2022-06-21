using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class LogManager
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Trace(string strMsg)
        {
            _logger.Trace(strMsg);
        }

        public static void Debug(string strMsg)
        {
            _logger.Debug(strMsg);
        }

        public static void Info(string strMsg)
        {
            _logger.Info(strMsg);
        }

        public static void Warn(string strMsg)
        {
            _logger.Warn(strMsg);
        }

        public static void Error(string strMsg)
        {
            _logger.Error(strMsg);
        }

        public static void Fatal(string strMsg)
        {
            _logger.Fatal(strMsg);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="_info"></param>
        public static void WriteLog2DB(LogInfo _info)
        {
            WriteLog2DB(new List<LogInfo>() { _info });
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="_logs"></param>
        public static void WriteLog2DB(List<LogInfo> _logs)
        {
            using (CoreDBContext context = new CoreDBContext())
            {
                foreach (var item in _logs)
                {
                    //加入数据库
                    context.Logs.Add(new CoreDBModels.Log()
                    {
                        CreateTime = DateTime.Now,
                        Creator = UserGlobal.CurrUser.Id,
                        LogStr = item.LogStr,
                        LogType = item.LogType
                    });
                }

                context.SaveChanges();
            }
        }
    }
}
