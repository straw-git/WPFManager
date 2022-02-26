using CoreDBModels;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public abstract class BaseMainWindow : WindowX
    {
        /// <summary>
        /// 当前窗体中的插件
        /// </summary>
        public List<BasePlugins> CurrWindowPlugins = new List<BasePlugins>();


        public void EnableMainWindow(bool _enable)
        {
            IsEnabled = _enable;
        }

        public void MaskVisible(bool _visible)
        {
            IsMaskVisible = _visible;
        }

        /// <summary>
        /// 添加插件信息
        /// </summary>
        /// <param name="_models"></param>
        public void AddPluginModels(List<BasePlugins> _models)
        {
            foreach (var model in _models)
            {
                if (CurrWindowPlugins.Contains(model))
                {
                    continue;
                }
                CurrWindowPlugins.Add(model);
            }
            CurrWindowPlugins = CurrWindowPlugins.OrderBy(c => c.Order).ToList();//排序
            UpdateMenus();//触发更新导航
        }

        public abstract void ShowLeftMenu(bool _show);//显示、隐藏左侧导航
        public abstract void ShowTopMenu(bool _show);//显示、隐藏上面导航
        public abstract void ReLoadCurrTopMenu();//刷新当前导航
        public abstract void SetFrameSource(string _s);//设置Frame内页
        public abstract void UpdateMenus();//更新导航
    }
}
