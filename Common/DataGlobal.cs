using CoreDBModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DataGlobal
{
    /// <summary>
    /// 获取数据字典中的数据
    ///<para>该方法不适合组织架构 </para>
    /// </summary>
    /// <param name="_parentCode"></param>
    /// <returns></returns>
    public static List<SysDic> GetDic(string _parentCode)
    {
        using (CoreDBContext context = new CoreDBContext())
        {
            return context.SysDic.Where(c => c.ParentCode == _parentCode).ToList();
        }
    }
}
