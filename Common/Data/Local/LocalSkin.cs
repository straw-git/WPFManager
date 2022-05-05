
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data.Local
{
    /// <summary>
    /// 本地的所有皮肤
    /// </summary>
    public class LocalSkin
    {
        public class SkinModel
        {
            public int SkinId { get; set; }
            public string SkinName { get; set; }
            //Main
            public string SkinColor { get; set; }
            //主题反差色
            public string SkinOppositeColor { get; set; }
            //Pages
            public string LoginBgColor { get; set; }
            //Button
            public string ButtonBgColor { get; set; }
            public string ButtonHoverColor { get; set; }
            public string ButtonBorderColor { get; set; }
            //TextBox
            public string TextBoxBgColor { get; set; }
            public string TextBoxFocusColor { get; set; }
            public string TextBoxFocusedShadowColor { get; set; }
            //DataGrid
            public string DataGridSelectedColor { get; set; }
            public string DataGridHoverColor { get; set; }
            public string DataGridRowColor { get; set; }
            public string DataGridEnableRowColor { get; set; }
            //ComboBox
            public string ComboBoxBgColor { get; set; }
            public string ComboBoxSelectedColor { get; set; }
            public string ComboBoxHoverColor { get; set; }
            //CheckBox
            public string CheckBoxBgColor { get; set; }
            public string CheckBoxSelectedColor { get; set; }
            //TreeView
            public string TreeViewSelectedBgColor { get; set; }
            public string TreeViewSelectedHeaderColor { get; set; }
            //Pager
            public string PagerBgColor { get; set; }
            public string PagerHoverColor { get; set; }
            //ProgressBar
            public string ProgressBarBgColor { get; set; }
            public string ProgressBarFgColor { get; set; }
            //System
            public string MinButtonColor { get; set; }
            public string MinButtonFocusColor { get; set; }
            public string MaxButtonColor { get; set; }
            public string MaxButtonFocusColor { get; set; }
            public string CloseButtonColor { get; set; }
            public string CloseButtonFocusColor { get; set; }
        }

        static string fileName = "skin.json";

        public static List<SkinModel> skins = new List<SkinModel>();
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
            skins = JsonHelper.DeserializeJsonToList<SkinModel>(jsonStr);

            if (skins == null || skins.Count == 0)
            {
                skins = new List<SkinModel>();
                //如果没有 添加一个默认的
                SkinModel skin = new SkinModel();
                skin.SkinId = 1;
                skin.SkinName = "基础样式";

                //Main
                skin.SkinColor = "#C62F2F";
                skin.SkinOppositeColor = "#ffffff";
                //Pages
                skin.LoginBgColor = "LightCyan";
                //Button
                skin.ButtonBgColor = "Black";
                skin.ButtonHoverColor = "#C62F2F";
                skin.ButtonBorderColor = "#FFCA3333";
                //TextBox 
                skin.TextBoxBgColor = "White";
                skin.TextBoxFocusColor = "#C62F2F";
                skin.TextBoxFocusedShadowColor = "AliceBlue";
                //DataGrid
                skin.DataGridHoverColor = "LightGray";
                skin.DataGridSelectedColor = "#C62F2F";
                skin.DataGridRowColor = "#FFCA3333";
                skin.DataGridEnableRowColor = "#FFCA3333";
                //ComboBox
                skin.ComboBoxBgColor = "White";
                skin.ComboBoxSelectedColor = "#FFCA3333";
                skin.ComboBoxHoverColor = "#FFCA3333";
                //CheckBox
                skin.CheckBoxBgColor = "White";
                skin.CheckBoxSelectedColor = "Blue";
                //TreeView
                skin.TreeViewSelectedBgColor = "LightGray";
                skin.TreeViewSelectedHeaderColor = "#C62F2F";
                //Pager
                skin.PagerBgColor = "#C62F2F";
                skin.PagerHoverColor = "#C62F2F";
                //ProgressBar
                skin.ProgressBarBgColor = "White";
                skin.ProgressBarFgColor = "Blue";
                //System
                skin.MinButtonColor = "#EAEAEA";
                skin.MinButtonFocusColor = "#FFFFFF";
                skin.MaxButtonColor = "#EAEAEA";
                skin.MaxButtonFocusColor = "#FFFFFF";
                skin.CloseButtonColor = "#EAEAEA";
                skin.CloseButtonFocusColor = "#FFFFFF";

                skins.Add(skin);
                Save();
            }
        }

        public static SkinModel GetModelById(int _skinId)
        {
            SkinModel model = new SkinModel();

            SkinModel _model = skins.Find(c => c.SkinId == _skinId);

            ModelCommon.CopyPropertyToModel(_model, ref model);

            return model;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="_model"></param>
        public static void Update(SkinModel _model)
        {
            SkinModel model = skins.Single(c => c.SkinId == _model.SkinId);
            ModelCommon.CopyPropertyToModel(_model, ref model, new List<string>() { "SkinId" });

            Save();
        }

        public static bool Exists(string _name)
        {
            return skins.Exists(c => c.SkinName == _name);
        }

        public static void Insert(SkinModel _model)
        {
            _model.SkinId = skins.Max(c => c.SkinId) + 1;
            skins.Insert(0, _model);
            Save();
        }

        public static void Remove(SkinModel _model)
        {
            if (_model.SkinId == 1) return;
            if (_model.SkinId == LocalSettings.settings.SkinId)
            {
                LocalSettings.UpdateSkinId(1);
            }
            skins.Remove(_model);
            Save();
        }

        private static void Save()
        {
            string jsonStr = JsonHelper.SerializeObject(skins);
            _helper.SaveNew(fileName, jsonStr);
        }
    }
}
