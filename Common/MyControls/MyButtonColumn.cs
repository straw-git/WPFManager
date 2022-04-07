

using System.Collections.Generic;

namespace Common.MyControls
{
    /// <summary>
    /// 按钮列
    /// </summary>
    public class MyButtonColumn
    {
        public string Header { get; set; }
        public int Width { get; set; }
        public List<MyDataGridColumnButton> Buttons { get; set; }
    }
}
