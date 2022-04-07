
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewPlugins
{
    public class RoleTreeViewCommon
    {
        TreeView tv;
        public RoleTreeViewCommon(TreeView _tv)
        {
            tv = _tv;
        }

        /// <summary>
        /// 加载角色列表
        /// </summary>
        public void Init(bool _showAllItem = true, bool _focusZero = false,int _selectedId=0)
        {
            tv.Items.Clear();
            TreeViewItem fristItem = new TreeViewItem();
            if (_showAllItem)
            {

                fristItem.Header = "全部角色";
                fristItem.Margin = new Thickness(2, 0, 0, 2);
                fristItem.Padding = new Thickness(10, 0, 0, 0);
                fristItem.Background = Brushes.Transparent;
                fristItem.Tag = 0;
                fristItem.IsSelected = false;
                tv.Items.Add(fristItem);
            }

            using (CoreDBContext context = new CoreDBContext())
            {
                var roles = context.SysDic.Where(c => c.ParentCode == DicData.Role).ToList();
                for (int i = 0; i < roles.Count; i++)
                {
                    var jobpost = roles[i];
                    TreeViewItem item = new TreeViewItem();
                    item.Header = jobpost.Name;
                    item.Margin = new Thickness(2, 0, 0, 2);
                    item.Padding = new Thickness(10, 0, 0, 0);
                    item.Background = Brushes.Transparent;
                    item.Tag =jobpost.Id;
                    item.IsSelected = jobpost.Id == _selectedId;
                    tv.Items.Add(item);
                }
            }

            if (_focusZero) fristItem.IsSelected = true;
        }
    }
}
