using DBModels.Sys;
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

namespace Common.Windows
{
    /// <summary>
    /// SelectedStore.xaml 的交互逻辑
    /// </summary>
    public partial class SelectedDateTime : Window
    {
        /// <summary>
        /// 选中的时间
        /// </summary>
        public DateTime SelectedDate = DateTime.Now;
        public bool Succeed = false;

        /// <summary>
        /// 选择时间
        /// </summary>
        /// <param name="_min">最小</param>
        /// <param name="_max">最大</param>
        /// <param name="_curr">当前</param>
        public SelectedDateTime(DateTime _min, DateTime _max, DateTime _curr)
        {
            InitializeComponent();
            this.UseCloseAnimation();

            cDate.MinDate = _min;
            cDate.MaxDate = _max;
            cDate.SelectedDate = _curr;
        }

        private void Calendar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedDate = cDate.SelectedDate;
            Succeed = true;
            Close();
        }
    }
}
