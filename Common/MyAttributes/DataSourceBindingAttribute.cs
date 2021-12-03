using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Common.MyAttributes
{
    /// <summary>
    /// 数据源绑定
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DataSourceBindingAttribute : Attribute
    {
        /// <summary>
        /// 列标题
        /// </summary>
        public string ColumnHeader = "";
        /// <summary>
        /// 宽度 数字 -1=*  | 0=Auto 
        /// </summary>
        public int Width = 0;
        /// <summary>
        /// 即是排序 也是相对的下标索引
        /// </summary>
        public int Index = 0;
        /// <summary>
        /// 绑定的数据列名称
        /// </summary>
        public string BindingName = "";
        /// <summary>
        /// 在列表中是否显示
        /// </summary>
        public Visibility ColumnVisibilitity = Visibility.Visible;

        /// <summary>
        /// 数据源绑定
        /// </summary>
        /// <param name="_columnHeader">列标题</param>
        /// <param name="_width">宽度 只能是数字 -1=*  | 0=Auto   </param>
        /// <param name="_index">即是排序 也是相对的下标索引 默认0</param>
        public DataSourceBindingAttribute(string _columnHeader, int _width = -1, int _index = 0)
        {
            ColumnHeader = _columnHeader;
            Width = _width;
            Index = _index;
        }
    }
}
