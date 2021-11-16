
using Panuon.UI.Silver;

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
}
