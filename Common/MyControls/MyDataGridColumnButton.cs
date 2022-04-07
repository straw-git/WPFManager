

using Panuon.UI.Silver;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Common.MyControls
{
    /// <summary>
    /// 列表中的按钮对应信息
    /// </summary>
    public class MyDataGridColumnButton 
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 字体 默认FontAwesome
        /// </summary>
        public string FontFamily { get; set; }
        /// <summary>
        /// 提示
        /// </summary>
        public string ToolTip { get; set; }
        /// <summary>
        /// 按钮样式 默认ButtonStyle.Link
        /// </summary>
        public ButtonStyle ButtonStyle { get; set; }
        /// <summary>
        /// 默认new Thickness(20, 0, 0, 0)
        /// </summary>
        public Thickness Margin { get; set; }
        /// <summary>
        /// 绑定的列表项 
        /// </summary>
        public string TagBindingName { get; set; }
        /// <summary>
        /// 字体 默认20d
        /// </summary>
        public double FontSize { get; set; }
        /// <summary>
        /// 前景
        /// </summary>
        public Color Foreground { get; set; }
        /// <summary>
        /// 鼠标滑过
        /// </summary>
        public Color Hoverground { get; set; }
        /// <summary>
        /// 按钮点击事件
        /// </summary>
        public RoutedEventHandler ClickHandler { get; set; }

        public MyDataGridColumnButton()
        {
            FontFamily = "FontAwesome";
            ButtonStyle = ButtonStyle.Link;
            Margin = new Thickness(20, 0, 0, 0);
            FontSize = 20d;
        }
    }

}
