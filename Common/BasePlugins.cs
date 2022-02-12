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
        /// 插件名称
        /// </summary>
        public string Name = string.Empty;
        /// <summary>
        /// 插件dll名称
        /// </summary>
        public string DLLName = string.Empty;
        /// <summary>
        /// 存放Page的文件夹名称 以,分割
        /// </summary>
        public string PageFolderNames = "";
        /// <summary>
        /// Logo图片名称
        /// </summary>
        public string LogoImageName = "logo.jpg";
        /// <summary>
        /// 导航介绍类名
        /// </summary>
        public string MenuClassName = "MenuInfo";

        /// <summary>
        /// 添加页面文件夹
        /// </summary>
        /// <param name="_name"></param>
        public void AddPageFolderName(string _name)
        {
            if (_name.IsNullOrEmpty()) return;
            PageFolderNames += $",{_name}";
        }

        /// <summary>
        /// 清空页面文件夹
        /// </summary>
        public void ClearFolderNames()
        {
            PageFolderNames = "";
        }

        /// <summary>
        /// 移除页面文件夹
        /// </summary>
        /// <param name="_name"></param>
        public void RemoveFolderName(string _name)
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
