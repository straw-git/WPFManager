
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Common.Utils
{
    public class JobPostTreeViewCommon
    {
        TreeView tvJobPost;
        public JobPostTreeViewCommon(TreeView _tvJobPost)
        {
            tvJobPost = _tvJobPost;
        }

        /// <summary>
        /// 加载职务列表
        /// </summary>
        public void Init(bool _showAllItem = true, bool _focusZero = false, int _selectedId = 0)
        {
            tvJobPost.Items.Clear();

            TreeViewItem fristItem = new TreeViewItem();

            if (_showAllItem)
            {
                fristItem.Header = "全部组织";
                fristItem.Margin = new Thickness(2, 0, 0, 2);
                fristItem.Padding = new Thickness(10, 0, 0, 0);
                fristItem.Background = Brushes.Transparent;
                fristItem.Tag = 0;
                fristItem.IsSelected = false;
                tvJobPost.Items.Add(fristItem);
            }

            using (CoreDBContext context = new CoreDBContext())
            {
                var jobPosts = context.SysDic.Where(c => c.ParentCode == DicData.JobPost).ToList();
                for (int i = 0; i < jobPosts.Count; i++)
                {
                    var jobpost = jobPosts[i];
                    TreeViewItem item = new TreeViewItem();
                    item.Header = jobpost.Name;
                    item.Margin = new Thickness(2, 0, 0, 2);
                    item.Padding = new Thickness(10, 0, 0, 0);
                    item.Background = Brushes.Transparent;
                    item.Tag = -1;
                    item.IsSelected = false;
                    tvJobPost.Items.Add(item);
                    GetChildItem(jobpost.QuickCode, item);
                }
            }
            if (_focusZero) fristItem.IsSelected = true;
        }

        private void GetChildItem(string _parentCode, TreeViewItem _parentItem, int _selectedId = 0)
        {
            using (var context = new CoreDBContext())
            {
                var dics = context.SysDic.Where(c => c.ParentCode == _parentCode);
                if (dics == null || dics.Count() == 0) return;
                foreach (var dic in dics)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = dic.Name;
                    item.Margin = new Thickness(2, 0, 0, 2);
                    item.Padding = new Thickness(10, 0, 0, 0);
                    item.Background = Brushes.Transparent;
                    item.Tag = dic.Id;
                    item.IsSelected = dic.Id == _selectedId;
                    _parentItem.Items.Add(item);
                    GetChildItem(dic.QuickCode, item);
                }
            }
        }
    }
}
