using System.Collections.Generic;
using System.Linq;

namespace Common
{
    /// <summary>
    /// 插件基类（在插件根目录PluginsInfo.cs中实现）
    /// </summary>
    public class BasePlugins
    {
        /// <summary>
        /// 插件
        /// </summary>
        public static string Code = string.Empty;
        /// <summary>
        /// 存放Page的文件夹名称 以,分割
        /// </summary>
        public static string PageFolderNames = "";

        /// <summary>
        /// 添加页面文件夹
        /// </summary>
        /// <param name="_name"></param>
        public static void AddPageFolderName(string _name)
        {
            if (_name.IsNullOrEmpty()) return;
            PageFolderNames += $",{_name}";
        }

        /// <summary>
        /// 清空页面文件夹
        /// </summary>
        public static void ClearFolderNames()
        {
            PageFolderNames = "";
        }

        /// <summary>
        /// 移除页面文件夹
        /// </summary>
        /// <param name="_name"></param>
        public static void RemoveFolderName(string _name)
        {
            List<string> folderNameList = PageFolderNames.Split(',').ToList();
            if (folderNameList.Contains(_name))
            {
                folderNameList.Remove(_name);
            }
            else return;

            PageFolderNames = "";
            foreach (var fName in folderNameList)
            {
                PageFolderNames += $",{fName}";
            }

            //如果前面带符号 移除
            if (PageFolderNames.IndexOf(',') == 0) PageFolderNames = PageFolderNames.Substring(1);
        }
    }
}
