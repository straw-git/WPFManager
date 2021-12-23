
namespace Common
{
    public class BaseMenuInfo
    {
        public string Name = "";
        public string FullName = "";
        public string Code = "";
        /// <summary>
        /// 内部排序
        /// </summary>
        public int SelfOrder = 0;
        
        public BaseMenuInfo(string name)
        {
            FullName = GetType().FullName.Replace($".{GetType().Name}", "");
            Name = name;//名称
            Code = FullName.Substring(FullName.LastIndexOf('.') + 1);//文件夹名称
        }
    }
}
