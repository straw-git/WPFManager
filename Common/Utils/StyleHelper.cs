
using Common.Data.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Client.Helper
{
    /// <summary>
    /// 样式帮助
    /// </summary>
    public class StyleHelper
    {
        public static void Init()
        {
            UpdateSkin();
        }

        /// <summary>
        /// 获取当前使用中的皮肤
        /// </summary>
        /// <returns></returns>
        public static void UpdateSkin()
        {
            var currUseSkinModel = LocalSkin.GetModelById(LocalSettings.settings.SkinId);

            Dictionary<string, object> items = GetModelProperties(currUseSkinModel);

            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item.Key != "SkinId" && item.Key != "SkinName")
                    {
                        if (item.Value != null && !string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            Application.Current.Resources.Remove(item.Key);
                            if (item.Key == "TextBoxFocusedShadowColor")
                            {
                                Application.Current.Resources.Add(item.Key, ConvertToColor(item.Value.ToString()));
                            }
                            else
                            {
                                Application.Current.Resources.Add(item.Key, ConvertToSolidColorBrush(item.Value.ToString()));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将#xxxxxx类型转换为SolidColorBrush
        /// </summary>
        /// <param name="_colorStr"></param>
        /// <returns></returns>
        public static SolidColorBrush ConvertToSolidColorBrush(string _colorStr)
        {
            return new SolidColorBrush(ConvertToColor(_colorStr));
        }

        /// <summary>
        /// 将#xxxxxx类型转换为Color
        /// </summary>
        /// <param name="_colorStr"></param>
        /// <returns></returns>
        public static Color ConvertToColor(string _colorStr)
        {
            return (Color)ColorConverter.ConvertFromString(_colorStr);
        }

        /// <summary>
        /// 获取类的所有属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetModelProperties<T>(T t)
        {
            if (t == null)
            {
                return null;
            }
            Dictionary<string, object> dic = new Dictionary<string, object>();

            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (properties.Length <= 0)
            {
                return null;
            }

            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(t, null);
                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    dic.Add(name, value);
                }
            }

            return dic;
        }
    }
}
