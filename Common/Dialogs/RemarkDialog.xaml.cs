using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Common.Dialogs
{
    /// <summary>
    /// DeleteDepartmentAny.xaml 的交互逻辑
    /// </summary>
    public partial class RemarkDialog : Window
    {
        /// <summary>
        /// 确认带有备注的删除模式窗口
        /// </summary>
        /// <param name="_dialogHeader"></param>
        public RemarkDialog(string _dialogHeader)
        {
            InitializeComponent();

            gbDelete.Header = _dialogHeader;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// 获取输入的备注字符串
        /// </summary>
        /// <returns></returns>
        public string GetRemark() 
        {
            string remark= txtDeleteRemark.Text;
            if (string.IsNullOrEmpty(remark)) return "无";
            return remark;
        }
    }
}
