using Common.Utils;
using System;
using System.Net;
using System.Windows;

namespace Common
{
    /// <summary>
    /// 下载插件DLL
    /// </summary>
    public static class PluginsDownload
    {
        /// <summary>
        /// 下载至Debug文件夹
        /// </summary>
        /// <param name="_baseUrl"></param>
        /// <param name="_dllName"></param>
        public static void ToPluginsFolder(string _baseUrl, string _dllName)
        {
            if (_baseUrl.IsNullOrEmpty() || _dllName.IsNullOrEmpty()) return;
            _baseUrl = _baseUrl.Trim();//去除空格
            try
            {
                string dllUrl = $"{AppDomain.CurrentDomain.BaseDirectory}{_dllName}";
                if (!FileUtils.IsFileIsUsed(dllUrl))//判断文件是否被占用
                    using (WebClient webClient = new WebClient())
                        //将文件下载至目标文件夹
                        webClient.DownloadFile($"{_baseUrl}{_dllName}", dllUrl);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
    }
}
