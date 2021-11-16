
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Panuon.UI.Silver;
using Common;
using Common.Entities;
using System.IO;
using Common.Data.Local;

namespace Client.Pages
{
    /// <summary>
    /// 用户权限
    /// </summary>
    public class MenuManager
    {
        /// <summary>
        /// 所有导航
        /// </summary>
        public static Dictionary<BaseMenuInfo, List<MenuItemModel>> Dic = new Dictionary<BaseMenuInfo, List<MenuItemModel>>();

        /// <summary>
        /// 初始化导航
        /// </summary>
        public static void InitMenus()
        {
            Dic.Clear();
            var plugins = LocalPlugins.Models.OrderBy(c => c.Order).ToList();
            foreach (var m in plugins)
            {
                FindMenuDLLPages(m.DLLPageName);
            }
        }

        private static void FindMenuDLLPages(string _nsp)
        {
            var nArr = _nsp.Split('.');
            if (nArr.Length != 2)
            {
                Notice.Show("DLL格式不正确,请将页面放置在[ dll命名空间.页面命名空间.导航命名空间 ]下", $"{_nsp}加载失败");
                return;
            }
            string fName = nArr[0];
            string lName = nArr[1];

            if (fName != "Client" && !File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}plugins\\{fName}.dll")) { return; }

            Assembly currAssembly = fName == "Client" ? Assembly.GetExecutingAssembly() : Assembly.LoadFrom($"plugins\\{fName}.dll");
            //查找所有页面的命名空间
            var pageNsps = (from t in currAssembly.GetTypes()
                            where t.IsClass && t.Namespace != null
                            && t.Namespace != _nsp
                            && t.Namespace.StartsWith(_nsp)
                            && !t.Namespace.StartsWith("<")
                            select t.Namespace).GroupBy(c => c).ToList();
            foreach (var nsp in pageNsps)
            {
                var items = (from t in currAssembly.GetTypes()
                             where t.IsClass && t.Namespace != null
                             && t.Namespace == nsp.Key
                             select t.FullName).ToList();

                BaseMenuInfo menuInfo = null;
                if (!items.Any(c => c.EndsWith("MenuInfo")))
                {
                    //没有导航说明文件的 不参与
                    continue;
                }
                if (fName == "Client")
                {
                    menuInfo = (BaseMenuInfo)Activator.CreateInstance(Type.GetType(items.Find(c => c.EndsWith("MenuInfo"))));
                }
                else
                {
                    menuInfo = (BaseMenuInfo)Activator.CreateInstance(currAssembly.GetType(items.Find(c => c.EndsWith("MenuInfo"))));
                }


                Dic.Add(menuInfo, new List<MenuItemModel>());
                List<BasePage> pages = new List<BasePage>();
                try
                {
                    foreach (var item in items)
                    {
                        if (item.EndsWith("MenuInfo")) continue;

                        Type itemObj = fName == "Client" ? Type.GetType(item) : currAssembly.GetType(item);
                        BasePage itemPage = (BasePage)Activator.CreateInstance(itemObj);
                        if (!itemPage.IsMenu) continue;//不是导航 排除

                        itemPage.Code = $"{item.Substring(item.LastIndexOf('.') + 1)}";

                        pages.Add(itemPage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (pages.Count > 1)
                {
                    //导航排序
                    pages = pages.OrderBy(c => c.Order).ToList();
                }
                foreach (var itemPage in pages)
                {
                    Grid itemGrid = itemPage.Content as Grid;
                    //已经获取到页
                    MenuItemModel itemModel = new MenuItemModel();
                    itemModel.Buttons = GetButtons(itemGrid);
                    itemModel.ParentCode = menuInfo.Code;
                    itemModel.Code = itemPage.Code;
                    itemModel.Name = itemPage.Title;
                    itemModel.Order = 0;
                    itemModel.Url = fName == "Client"
                        ? $"/{lName}/{menuInfo.Code}/{itemModel.Code}.xaml"
                        : $"pack://application:,,,/{fName};component/{lName}/{menuInfo.Code}/{itemModel.Code}.xaml";

                    Dic[menuInfo].Add(itemModel);
                }
            }
        }

        /// <summary>
        /// 递归查找按钮
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private static List<MenuItemButtonModel> GetButtons(Visual control)
        {
            List<MenuItemButtonModel> list = new List<MenuItemButtonModel>();

            int childCount = VisualTreeHelper.GetChildrenCount(control);

            for (int i = 0; i < childCount; i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(control, i);
                if (childVisual.GetType() == typeof(GroupBox))
                {
                    #region 特殊的GroupBox 

                    var groupBox = (childVisual as GroupBox);
                    //从依赖项中 读取gropubox右上角的按钮
                    var element = GroupBoxHelper.GetExtendControl(groupBox);
                    if (element != null && VisualTreeHelper.GetChildrenCount(element) > 0)
                    {
                        list.AddRange(GetButtons(element));
                    }
                    //读取groupbox的内容
                    var grid = (childVisual as GroupBox).Content as Grid;
                    list.AddRange(GetButtons(grid));

                    #endregion

                }
                else if (childVisual.GetType() == typeof(Button))
                {
                    Button button = childVisual as Button;
                    MenuItemButtonModel model = new MenuItemButtonModel();
                    model.Content = button.Content == null ? "未设置内容" : button.Content.ToString();
                    model.Name = button.Name;

                    list.Add(model);
                }
                else
                {
                    #region 常规容器

                    if (VisualTreeHelper.GetChildrenCount(childVisual) > 0)
                    {
                        list.AddRange(GetButtons(childVisual));
                    }

                    #endregion 
                }
            }

            return list;
        }

        private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }

    }
}
