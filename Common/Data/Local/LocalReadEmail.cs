using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Local
{
    public class LocalReadEmail
    {
        public class ReadEmailModel
        {
            /// <summary>
            /// 已经阅读的角色邮件
            /// </summary>
            public string RoleEmailIds { get; set; }
            /// <summary>
            /// 已经阅读的全部用户邮件
            /// </summary>
            public string AllUserEmailIds { get; set; }
        }

        static string fileName = "";

        public static ReadEmailModel model;
        static LocalFileHelper _helper = new LocalFileHelper(LocalFileHelper.LocalFileDicType.LocalData, "LocalSetting");

        static LocalReadEmail()
        {
            fileName = $"{UserGlobal.CurrUser.Id}-reademails.json";
            ReadJson();
        }

        /// <summary>
        /// 读取json文件
        /// </summary>
        public static void ReadJson()
        {
            string jsonStr = _helper.ReadFile(fileName);
            model = JsonHelper.DeserializeJsonToObject<ReadEmailModel>(jsonStr);

            if (model == null)
            {
                //如果没有 添加一个默认的
                model = new ReadEmailModel();
                model.AllUserEmailIds = "";
                model.RoleEmailIds = "";

                Save();
            }
        }

        public static List<string> GetAllUserEmail() 
        {
            return model.AllUserEmailIds.Split(',').ToList();
        }

        public static List<string> GetRoleEmail() 
        {
            return model.RoleEmailIds.Split(',').ToList();
        }

        /// <summary>
        /// 读一个所有用户的邮件
        /// </summary>
        /// <param name="_emailSendToId"></param>
        public static void ReadAllUserEmail(int _emailSendToId)
        {
            if (model.AllUserEmailIds.IsNullOrEmpty()) 
                model.AllUserEmailIds = _emailSendToId.ToString();
            else 
                model.AllUserEmailIds += $",{_emailSendToId}";

            Save();
        }

        /// <summary>
        /// 读一个发给角色的邮件
        /// </summary>
        /// <param name="_emailSendToId"></param>
        public static void ReadRoleEmail(int _emailSendToId) 
        {
            if (model.RoleEmailIds.IsNullOrEmpty())
                model.RoleEmailIds = _emailSendToId.ToString();
            else
                model.RoleEmailIds += $",{_emailSendToId}";

            Save();
        }

        private static void Save()
        {
            string jsonStr = JsonHelper.SerializeObject(model);
            _helper.SaveNew(fileName, jsonStr);
        }
    }
}
