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
        static WebClient webClient;
        static PluginsDownload()
        {
            webClient = new WebClient();
        }

        /// <summary>
        /// 下载至Debug文件夹
        /// </summary>
        /// <param name="_baseUrl"></param>
        /// <param name="_dllName"></param>
        public static void ToPluginsFolder(string _baseUrl, string _dllName)
        {
            if (_baseUrl.IsNullOrEmpty() || _dllName.IsNullOrEmpty()) return;
            _baseUrl = _baseUrl.Trim();//去除空格
            if (_baseUrl.EndsWith("/")) _baseUrl = _baseUrl.Substring(0, _baseUrl.Length - 1);//如果带斜杠结尾的话 将斜杠去掉
            try
            {
                //将文件下载至目标文件夹
                webClient.DownloadFile($"{_baseUrl}/{_dllName}.dll", $"{AppDomain.CurrentDomain.BaseDirectory}{_dllName}.dll");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
