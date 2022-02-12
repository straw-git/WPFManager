
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Local
{
    public class LocalPlugins
    {
        public class DBModel
        {
            public string DLLPageName { get; set; }
            public int Order { get; set; }
        }

        static string fileName = "plugins.json";

        public static List<DBModel> Models=new List<DBModel>();
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
            Models = JsonHelper.DeserializeJsonToList<DBModel>(jsonStr);

            if (Models == null||Models.Count==0)
            {
                Models = new List<DBModel>();
                Models.Add(new DBModel() { DLLPageName = "Client",Order=0 });

                Save();
            }
        }

        public static void Save()
        {
            string jsonStr = JsonHelper.SerializeObject(Models);
            _helper.SaveNew(fileName, jsonStr);
        }
    }
}
