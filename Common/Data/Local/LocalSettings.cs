
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Local
{
    public class LocalSettings
    {
        public class SettingsModel
        {
            /// <summary>
            /// 使用中的皮肤Id
            /// </summary>
            public int SkinId { get; set; }

            /// <summary>
            /// 主窗体标题
            /// </summary>
            public string MainWindowTitle { get; set; }

            public string CompanyName { get; set; }

            public string Versions { get; set; }

            /// <summary>
            /// 小数点位数
            /// </summary>
            public int SuffixNumber { get; set; }
        }

        static string fileName = "settings.json";

        public static SettingsModel settings;
        static LocalFileHelper _helper = new LocalFileHelper(LocalFileHelper.LocalFileDicType.LocalData, "LocalSetting");

        public static void Init()
        {
            ReadJson();
        }

        /// <summary>
        /// 读取json文件
        /// </summary>
        public static void ReadJson()
        {
            string jsonStr = _helper.ReadFile(fileName);
            settings = JsonHelper.DeserializeJsonToObject<SettingsModel>(jsonStr);

            if (settings == null)
            {
                //如果没有 添加一个默认的
                settings = new SettingsModel();
                settings.SkinId = 1;

                settings.MainWindowTitle = "";
                settings.CompanyName = "";
                settings.Versions = "1.0";
                settings.SuffixNumber = 3;

                Save();
            }
        }

        /// <summary>
        /// 更改皮肤Id
        /// </summary>
        /// <param name="_skinId"></param>
        public static void UpdateSkinId(int _skinId)
        {
            if (settings.SkinId == _skinId) return;

            settings.SkinId = _skinId;
            Save();
        }

        public static void UpdateMainTitle(string mainTitle)
        {
            if (settings.MainWindowTitle == mainTitle) return;

            settings.MainWindowTitle = mainTitle;
            Save();
        }

        public static void UpdateCompanyName(string _companyName) 
        {
            if (settings.CompanyName == _companyName) return;

            settings.CompanyName = _companyName;

            Save();
        }

        private static void Save()
        {
            string jsonStr = JsonHelper.SerializeObject(settings);
            _helper.SaveNew(fileName, jsonStr);
        }
    }
}
