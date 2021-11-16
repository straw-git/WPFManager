
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Local
{
    public class LocalServer
    {
        public class ServerModel
        {
            public bool ServerStart { get; set; }
            public string ServerIP { get; set; }
            public string ServerPort { get; set; }
        }

        static string fileName = "server.json";

        public static ServerModel Model;
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
            Model = JsonHelper.DeserializeJsonToObject<ServerModel>(jsonStr);

            if (Model == null)
            {
                //如果没有 添加一个默认的
                Model = new ServerModel();
                Model.ServerStart = false;
                Model.ServerIP = "";
                Model.ServerPort = "";

                Save();
            }
        }

        public static void Update(ServerModel _model)
        {
            Model.ServerStart = _model.ServerStart;
            Model.ServerIP = _model.ServerIP;
            Model.ServerPort = _model.ServerPort;
            Save();
        }

        private static void Save()
        {
            string jsonStr = JsonHelper.SerializeObject(Model);
            _helper.SaveNew(fileName, jsonStr);
        }
    }
}
