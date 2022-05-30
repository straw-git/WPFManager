using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Common.MyControls
{
    /// <summary>
    /// SuperRichTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class SuperRichTextBox : UserControl
    {
        System.Threading.Timer timer;
        /// <summary>
        /// 时间格式化字符串
        /// </summary>
        public string TimerFormatStr = "yyyy年MM月dd日 HH:mm:ss";


        public SuperRichTextBox()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        }

        #region Private

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtbEditor.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && temp.Equals(FontWeights.Bold);
            temp = rtbEditor.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && temp.Equals(FontStyles.Italic);
            temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && temp.Equals(TextDecorations.Underline);
            temp = rtbEditor.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;
            temp = rtbEditor.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            cmbFontSize.Text = temp.ToString();
        }
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
            }
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
            }
        }
        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }
        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            double size = 0;
            if (!double.TryParse(cmbFontSize.Text, out size))
            {
                return;
            }
            rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, size);
        }

        #endregion

        #region Public

        /// <summary>
        /// 开始计时
        /// </summary>
        public void StartTime()
        {
            timer = new System.Threading.Timer(OnTimer, null, 0, 1000);
        }
        private void OnTimer(object state)
        {
            UIGlobal.RunUIAction(() =>
            {
                lblCurrTime.Content = DateTime.Now.ToString(TimerFormatStr);
            });
        }

        public void SetTime(string _timerStr)
        {
            lblCurrTime.Content = _timerStr;
        }

        /// <summary>
        /// 获取Text
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            //创建一个流
            MemoryStream s = new MemoryStream();
            //获得富文本中的内容
            TextRange documentTextRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            //将富文本中的内容转换成xaml的格式，并保存到指定的流中
            documentTextRange.Save(s, DataFormats.XamlPackage);
            //将流中的内容转换成字节数组，并转换成base64的等效格式
            return Convert.ToBase64String(s.ToArray());
        }

        /// <summary>
        /// 设置Text
        /// </summary>
        /// <param name="_text"></param>
        public void SetText(string _text)
        {
            MemoryStream s = new MemoryStream(Convert.FromBase64String(Convert.ToString(_text)));
            TextRange TR = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            TR.Load(s, DataFormats.XamlPackage);
        }

        #endregion

    }
}
