
namespace Common
{
    public class BaseMenuInfo
    {
        public string Name = "";
        public string FullName = "";
        public string Code = "";
        public string Icon = "";
        /// <summary>
        /// 内部排序
        /// </summary>
        public int SelfOrder = 0;
        
        public BaseMenuInfo(string name,string icon= "\xf260")
        {
            FullName = GetType().FullName.Replace($".{GetType().Name}", "");
            Name = name;//名称
            Code = FullName.Substring(FullName.LastIndexOf('.') + 1);//文件夹名称
            Icon = icon;
        }
    }
}
