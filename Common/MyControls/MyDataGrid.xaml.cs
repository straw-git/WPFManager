using Common.MyAttributes;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Common.MyControls
{
    /// <summary>
    /// 页面状态
    /// </summary>
    public enum MyDataGridState
    {
        Loading,//加载中
        Normal,//正常
    }

    /// <summary>
    /// 分页样式
    /// </summary>
    public enum MyPagerSkin
    {
        ZHHans,//简体中文
        OnlyNumber//仅数字
    }

    /// <summary>
    /// MyPagerDataGrid.xaml 的交互逻辑
    /// </summary>
    public partial class MyDataGrid : UserControl
    {
        #region Properties

        public static readonly DependencyProperty HeaderMinHeightProperty = DependencyProperty.Register("HeaderMinHeight", typeof(double), typeof(MyDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty NoDataBorderMarginProperty = DependencyProperty.Register("NoDataBorderMargin", typeof(Thickness), typeof(MyDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty FirstPageButtonVisibilityProperty = DependencyProperty.Register("FirstPageButtonVisibility", typeof(Visibility), typeof(MyDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty LastPageButtonVisibilityProperty = DependencyProperty.Register("LastPageButtonVisibility", typeof(Visibility), typeof(MyDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectAllCheckBoxVisibilityProperty = DependencyProperty.Register("SelectAllCheckBoxVisibility", typeof(Visibility), typeof(MyDataGrid), new PropertyMetadata(null));


        /// <summary>
        /// 标题最低高度 
        /// </summary>
        public double HeaderMinHeight
        {
            get { return (double)GetValue(HeaderMinHeightProperty); }
            set
            {
                if (value == 0) SetValue(HeaderMinHeightProperty, 40);
                else SetValue(HeaderMinHeightProperty, value);
            }
        }

        /// <summary>
        /// 无数据Border边距
        /// </summary>
        public Thickness NoDataBorderMargin
        {
            get { return (Thickness)GetValue(NoDataBorderMarginProperty); }
            set { SetValue(NoDataBorderMarginProperty, value); }
        }

        /// <summary>
        /// 首页按钮显示状态
        /// </summary>
        public Visibility FirstPageButtonVisibility
        {
            get { return (Visibility)GetValue(FirstPageButtonVisibilityProperty); }
            set { SetValue(FirstPageButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// 尾页按钮显示状态
        /// </summary>
        public Visibility LastPageButtonVisibility
        {
            get { return (Visibility)GetValue(LastPageButtonVisibilityProperty); }
            set { SetValue(LastPageButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// 显示
        /// </summary>
        public Visibility SelectAllCheckBoxVisibility
        {
            get { return (Visibility)GetValue(SelectAllCheckBoxVisibilityProperty); }
            set { SetValue(SelectAllCheckBoxVisibilityProperty, value); }
        }

        #endregion

        #region Methods

        public delegate void OnStateChangedHandler(MyDataGrid _send, MyDataGridState _state);
        public event OnStateChangedHandler StateChanged;//状态更改事件

        public delegate void OnSelectedAllClickHandler(bool _isChecked);
        public event OnSelectedAllClickHandler SelectedAllClick;//全选点击事件

        #endregion

        /// <summary>
        /// 表格的列数据
        /// </summary>
        public List<DataSourceBindingAttribute> GridLinesData = new List<DataSourceBindingAttribute>();

        private MyDataGridState currState = MyDataGridState.Normal;
        /// <summary>
        /// 当前状态
        /// </summary>
        public MyDataGridState CurrSate
        {
            get { return currState; }
            set
            {
                currState = value;
                StateChanged?.Invoke(this, value);//触发状态更改事件
            }
        }

        private MyPagerSkin currPagerSkin = MyPagerSkin.ZHHans;
        /// <summary>
        /// 当前翻页样式
        /// </summary>
        public MyPagerSkin CurrPagerSkin
        {
            get { return currPagerSkin; }
            set
            {
                currPagerSkin = value;
                switch (value)
                {
                    case MyPagerSkin.ZHHans:
                        //简体中文
                        spZHHansSkin.Visibility = Visibility.Visible;
                        spOnlyNumberSkin.Visibility = Visibility.Collapsed;
                        break;
                    case MyPagerSkin.OnlyNumber:
                        //仅数字
                        spZHHansSkin.Visibility = Visibility.Collapsed;
                        spOnlyNumberSkin.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 选中的数据
        /// </summary>
        public List<dynamic> SelectedItems = new List<dynamic>();

        dynamic Data;
        public MyDataGrid()
        {
            InitializeComponent();

            HeaderMinHeight = 40;//默认40
            FirstPageButtonVisibility = Visibility.Visible;//首页按钮默认显示
            LastPageButtonVisibility = Visibility.Visible;//尾页按钮默认显示

            CurrPagerSkin = MyPagerSkin.ZHHans;//默认简体中文样式
            SelectAllCheckBoxVisibility = Visibility.Visible;//显示全选的复选框
        }

        /// <summary>
        /// 设置DataGrid 数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_source">数据源</param>
        /// <param name="_buttonTemplates">操作按钮列</param>
        /// <param name="_buttonColumnIndex">位置 默认-1 为最后一位</param>
        public void BindingDataGrid<T>(dynamic _source, MyButtonColumn _buttonTemplates = null, int _buttonColumnIndex = -1) where T : class, new()
        {
            #region 读取实体属性

            //获取对象所有属性
            PropertyInfo[] properties = new T().GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            //如果对象没有属性 直接退出
            if (properties.Length <= 0)
            {
                MessageBox.Show("当前实体类中没有发现 数据列属性");
                return;
            }

            foreach (PropertyInfo item in properties)
            {
                //属性名称
                string columnName = item.Name;
                // 获取指定属性的属性描述
                var bindingAtt = typeof(T).GetProperty(columnName).GetCustomAttribute<DataSourceBindingAttribute>();

                if (bindingAtt != null)//没有标记属性的值不显示
                {
                    bindingAtt.BindingName = columnName;
                    GridLinesData.Add(bindingAtt);
                }
            }
            GridLinesData = GridLinesData.OrderBy(c => c.Index).ToList();//对显示列进行排序

            #endregion

            #region 生成列

            foreach (var bindingAtt in GridLinesData)
            {
                //Edit DataGrid Columns
                if (list.Columns.Any(c => c.Header != null && c.Header.ToString() == bindingAtt.ColumnHeader))
                {
                    //如果DataGrid中有表头和数据中一样的 不显示
                    continue;
                }
                else
                {
                    //显示并绑定数据
                    DataGridColumn column = new DataGridTextColumn() { Header = bindingAtt.ColumnHeader, Binding = new Binding(bindingAtt.BindingName) };
                    if (bindingAtt.Width == -1)
                    {
                        column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    }
                    else if (bindingAtt.Width == 0)
                    {
                        column.Width = DataGridLength.Auto; //new DataGridLength(0, DataGridLengthUnitType.Auto);
                    }
                    else
                    {
                        column.Width = bindingAtt.Width;// new DataGridLength(bindingAtt.Width);
                    }
                    list.Columns.Insert(bindingAtt.Index, column);
                }
            }

            #endregion 

            #region 添加操作按钮列

            if (_buttonTemplates != null)
            {
                DataGridTemplateColumn dataGridTemplateColumn = new DataGridTemplateColumn() { Header = _buttonTemplates.Header };
                if (_buttonTemplates.Width == -1)
                {
                    dataGridTemplateColumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                }
                else if (_buttonTemplates.Width == 0)
                {
                    dataGridTemplateColumn.Width = DataGridLength.Auto; //new DataGridLength(0, DataGridLengthUnitType.Auto);
                }
                else
                {
                    dataGridTemplateColumn.Width = _buttonTemplates.Width;// new DataGridLength(bindingAtt.Width);
                }


                DataTemplate dataTemplate = new DataTemplate();
                FrameworkElementFactory factory = new FrameworkElementFactory(typeof(StackPanel));
                factory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

                for (int i = 0; i < _buttonTemplates.Buttons.Count; i++)
                {
                    var btnInfo = _buttonTemplates.Buttons[i];
                    FrameworkElementFactory btn = new FrameworkElementFactory(typeof(Button));
                    btn.SetValue(ContentProperty, btnInfo.Content);
                    btn.SetValue(FontFamilyProperty, FindResource(btnInfo.FontFamily));
                    btn.SetValue(ToolTipProperty, btnInfo.ToolTip);
                    btn.SetValue(MarginProperty, btnInfo.Margin);
                    btn.SetValue(ButtonHelper.ButtonStyleProperty, btnInfo.ButtonStyle);
                    btn.SetValue(TagProperty, new Binding(btnInfo.TagBindingName));
                    btn.SetValue(FontSizeProperty, btnInfo.FontSize);
                    btn.SetValue(ForegroundProperty, new SolidColorBrush(btnInfo.Foreground));
                    btn.SetValue(ButtonHelper.HoverBrushProperty, new SolidColorBrush(btnInfo.Hoverground));
                    btn.AddHandler(Button.ClickEvent, btnInfo.ClickHandler);

                    factory.AppendChild(btn);
                }

                dataTemplate.VisualTree = factory;
                dataGridTemplateColumn.CellTemplate = dataTemplate;

                if (_buttonColumnIndex > -1)
                    list.Columns.Insert(_buttonColumnIndex, dataGridTemplateColumn);
                else
                    list.Columns.Add(dataGridTemplateColumn);
            }

            #endregion 

            Data = _source;
            //绑定数据源
            list.ItemsSource = _source;
        }

        //数字导航模式 更改当前页码
        private void UpdateOnlyNumberPager(object sender, Panuon.UI.Silver.Core.CurrentIndexChangedEventArgs e)
        {

        }

        //简体中文导航模式 更改当前页码
        private void UpdateZHHansPager(int _pageIndex)
        {

        }

        //页码文本框
        private void txtZHHansCurrIndex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //点击了回车
                int pageIndex = 1;
                if (string.IsNullOrEmpty(txtZHHansCurrIndex.Text))
                {
                    txtZHHansCurrIndex.Text = "1";
                    txtZHHansCurrIndex.SelectAll();
                    return;
                }
                int.TryParse(txtZHHansCurrIndex.Text, out pageIndex);
                UpdateZHHansPager(pageIndex);
            }
        }

        //只允许输入数字
        private void txtZHHansCurrIndex_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)
            {
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        //全选、反选当页数据
        private void cbSelectListAll_Click(object sender, RoutedEventArgs e)
        {
            bool isCheck = (bool)(sender as CheckBox).IsChecked;
            for (int i = 0; i < Data.Count; i++)
            {
                Data[i].IsChecked = isCheck;
            }
            SelectedAllClick?.Invoke(isCheck);
        }

        private void list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                var selectItem = list.SelectedItem as BaseUIModel;
                selectItem.IsChecked = !selectItem.IsChecked;
            }
        }

        private void list_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Console.WriteLine();
        }
    }
}
