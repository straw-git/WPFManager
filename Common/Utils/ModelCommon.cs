using System.Collections.Generic;
using System.Linq;

namespace Common.Utils
{
    public class ModelCommon
    {
        /// <summary>
        /// 拷贝t中的属性值到baseT中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromModel">原属性</param>
        /// <param name="toModel">将要拷贝的属性</param>
        /// <param name="exceptNames">被排除在外的属性名称</param>
        public static void CopyPropertyToModel<T>(T fromModel, ref T toModel, List<string> exceptNames = null)
        {
            if (exceptNames == null) { exceptNames = new List<string>(); }
            //获取对象所有属性
            System.Reflection.PropertyInfo[] properties = fromModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            System.Reflection.PropertyInfo[] baseTProperties = toModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            //如果对象没有属性 直接退出
            if (properties.Length <= 0)
            {
                return;
            }

            foreach (System.Reflection.PropertyInfo item in properties)
            {
                //属性名称
                string name = item.Name;
                if (!exceptNames.Contains(name))
                {
                    //属性值
                    object value = item.GetValue(fromModel, null);
                    //设置为属性值
                    baseTProperties.Single(c => c.Name == name).SetValue(toModel, value);
                }
            }
        }
    }
}
