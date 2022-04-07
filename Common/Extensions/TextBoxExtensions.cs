
using Panuon.UI.Silver;
using System.Windows.Media;

public static class TextBoxExtensions
{
    public static bool IsEmpty(this System.Windows.Controls.TextBox sender, string info)
    {
        if (string.IsNullOrEmpty(sender.Text))
        {
            MessageBoxX.Show($"{info} 不能为空", "空值");
            sender.Focus();
            sender.SelectAll();
            return false;
        }
        return true;
    }

    #region 空值检查 CheckEmpty

    public static void Error(this System.Windows.Controls.TextBox sender)
    {
        sender.BorderBrush = new SolidColorBrush(Colors.Red);
    }

    public static void Normal(this System.Windows.Controls.TextBox sender) 
    {
        sender.BorderBrush = new SolidColorBrush(Colors.LightGray);
    }

    /// <summary>
    /// 空值检查
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    public static bool NotEmpty(this System.Windows.Controls.TextBox sender)
    {
        if (sender.Text.IsNullOrEmpty())
        {
            sender.Error();
            return false;
        }
        else 
        {
            sender.Normal();
            return true;
        }
    }

    #endregion 
}
