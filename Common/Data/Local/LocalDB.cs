
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Local
{
    public class LocalDB
    {
        public class DBModel
        {
            public string DataSource { get; set; }
            public string InitialCatalog { get; set; }
            public string UserId { get; set; }
            public string Password { get; set; }
        }

        static string fileName = "db.json";

        public static DBModel Model;
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
            Model = JsonHelper.DeserializeJsonToObject<DBModel>(jsonStr);

            if (Model == null)
            {
                //如果没有 添加一个默认的
                Model = new DBModel();
                Model.DataSource = "";
                Model.InitialCatalog = "";
                Model.Password = "";
                Model.UserId = "";

                Save();
            }
        }

        public static void Save()
        {
            string jsonStr = JsonHelper.SerializeObject(Model);
            _helper.SaveNew(fileName, jsonStr);
        }
    }
}
