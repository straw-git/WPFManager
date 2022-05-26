
using Common.Data.Local;
using Common.Utils;
using Panuon.UI.Silver;
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

namespace Client.Windows
{
    /// <summary>
    /// SkinWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SkinWindow : WindowX
    {
        public SkinWindow()
        {
            InitializeComponent();

            BindMainComobox();
            UpdateEditPage();

            this.UseCloseAnimation();
        }

        private void BtnSaveSkin_Click(object sender, RoutedEventArgs e)
        {
            LocalSkin.SkinModel focusModel = this.skin.SelectedItem as LocalSkin.SkinModel;
            LocalSettings.UpdateSkinId(focusModel.SkinId);

            StyleHelper.UpdateSkin();

            Notice.Show($"[{useSkin.SkinName}]主题使用成功", "提示", 3, MessageBoxIcon.Success);
            BindMainComobox();
            UpdateEditPage();
        }

        /// <summary>
        /// 更新下拉菜单
        /// </summary>
        private void BindMainComobox()
        {
            this.skin.ItemsSource = null;

            this.skin.ItemsSource = LocalSkin.skins;
            this.skin.DisplayMemberPath = "SkinName";
            this.skin.SelectedValuePath = "SkinId";

            this.skin.SelectedValue = LocalSettings.settings.SkinId.ToString();
        }

        LocalSkin.SkinModel useSkin = new LocalSkin.SkinModel();

        #region Button

        private void ButtonBgColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_ButtonBgColor");
            this.Resources.Add("_ButtonBgColor", buttonBgColor.SelectedBrush);
            useSkin.ButtonBgColor = buttonBgColor.Text;
        }

        private void ButtonHoverColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_ButtonHoverColor");
            this.Resources.Add("_ButtonHoverColor", buttonHoverColor.SelectedBrush);
            useSkin.ButtonHoverColor = buttonHoverColor.Text;
        }

        private void ButtonBorderColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_ButtonBorderColor");
            this.Resources.Add("_ButtonBorderColor", buttonBorderColor.SelectedBrush);
            useSkin.ButtonBorderColor = buttonBorderColor.Text;
        }

        #endregion

        #region SkinColor

        private void CpSkinColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_SkinColor");
            this.Resources.Add("_SkinColor", cpSkinColor.SelectedBrush);
            useSkin.SkinColor = cpSkinColor.Text;
        }


        private void cpSkinOppositeColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_SkinOppositeColor");
            this.Resources.Add("_SkinOppositeColor", cpSkinOppositeColor.SelectedBrush);
            useSkin.SkinOppositeColor = cpSkinOppositeColor.Text;
        }

        #endregion 

        #region TextBox

        private void TextboxBgColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_TextBoxBgColor");
            this.Resources.Add("_TextBoxBgColor", textboxBgColor.SelectedBrush);
            useSkin.TextBoxBgColor = textboxBgColor.Text;
        }

        private void TextboxFocusColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_TextBoxFocusColor");
            this.Resources.Add("_TextBoxFocusColor", textboxFocusColor.SelectedBrush);
            useSkin.TextBoxFocusColor = textboxFocusColor.Text;
        }

        private void TextboxFocusedShadowColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_TextBoxFocusedShadowColor");
            this.Resources.Add("_TextBoxFocusedShadowColor", textboxFocusedShadowColor.ShadowColor);
            useSkin.TextBoxFocusedShadowColor = textboxFocusedShadowColor.Text;
        }

        #endregion

        #region DataGrid

        private void DatagridSelectedColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_DataGridSelectedColor");
            this.Resources.Add("_DataGridSelectedColor", datagridSelectedColor.SelectedBrush);
            useSkin.DataGridSelectedColor = datagridSelectedColor.Text;
        }

        private void DatagridHoverColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_DataGridHoverColor");
            this.Resources.Add("_DataGridHoverColor", datagridHoverColor.SelectedBrush);
            useSkin.DataGridHoverColor = datagridHoverColor.Text;
        }

        private void DatagridRowColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_DataGridRowColor");
            this.Resources.Add("_DataGridRowColor", datagridRowColor.SelectedBrush);
            useSkin.DataGridRowColor = datagridRowColor.Text;
        }

        private void DatagridEnableRowColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_DataGridEnableRowColor");
            this.Resources.Add("_DataGridEnableRowColor", datagridEnableRowColor.SelectedBrush);
            useSkin.DataGridEnableRowColor = datagridEnableRowColor.Text;
        }


        #endregion

        #region ComoBox

        private void ComoboxBgColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_ComboBoxBgColor");
            this.Resources.Add("_ComboBoxBgColor", comoboxBgColor.SelectedBrush);
            useSkin.ComboBoxBgColor = comoboxBgColor.Text;
        }

        private void ComoboxSelectedColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_ComboBoxSelectedColor");
            this.Resources.Add("_ComboBoxSelectedColor", comoboxSelectedColor.SelectedBrush);
            useSkin.ComboBoxSelectedColor = comoboxSelectedColor.Text;
        }

        private void ComoboxHoverColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_ComboBoxHoverColor");
            this.Resources.Add("_ComboBoxHoverColor", comoboxHoverColor.SelectedBrush);
            useSkin.ComboBoxHoverColor = comoboxHoverColor.Text;
        }

        #endregion

        #region CheckBox

        private void CheckboxBgColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_CheckBoxBgColor");
            this.Resources.Add("_CheckBoxBgColor", checkboxBgColor.SelectedBrush);
            useSkin.CheckBoxBgColor = checkboxBgColor.Text;
        }

        private void CheckboxSelectedColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_CheckBoxSelectedColor");
            this.Resources.Add("_CheckBoxSelectedColor", checkboxSelectedColor.SelectedBrush);
            useSkin.CheckBoxSelectedColor = checkboxSelectedColor.Text;
        }

        #endregion

        #region TreeView

        private void TreeviewSelectedBgColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_TreeViewSelectedBgColor");
            this.Resources.Add("_TreeViewSelectedBgColor", treeviewSelectedBgColor.SelectedBrush);
            useSkin.TreeViewSelectedBgColor = treeviewSelectedBgColor.Text;
        }

        private void TreeviewSelectedHeaderColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_TreeViewSelectedHeaderColor");
            this.Resources.Add("_TreeViewSelectedHeaderColor", treeviewSelectedHeaderColor.SelectedBrush);
            useSkin.TreeViewSelectedHeaderColor = treeviewSelectedHeaderColor.Text;
        }

        #endregion

        #region Pager

        private void PagerBgColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_PagerBgColor");
            this.Resources.Add("_PagerBgColor", pagerBgColor.SelectedBrush);
            useSkin.PagerBgColor = pagerBgColor.Text;
        }

        private void PagerHoveColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_PagerHoverColor");
            this.Resources.Add("_PagerHoverColor", pagerHoveColor.SelectedBrush);
            useSkin.PagerHoverColor = pagerHoveColor.Text;
        }

        #endregion

        #region ProgressBar

        private void ProgressbarBgColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_ProgressBarBgColor");
            this.Resources.Add("_ProgressBarBgColor", progressbarBgColor.SelectedBrush);
            useSkin.ProgressBarBgColor = progressbarBgColor.Text;
        }

        private void ProgressbarFgColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_ProgressBarFgColor");
            this.Resources.Add("_ProgressBarFgColor", progressbarFgColor.SelectedBrush);
            useSkin.ProgressBarFgColor = progressbarFgColor.Text;
        }

        #endregion

        #region System

        private void MinbuttonColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_MinButtonColor");
            this.Resources.Add("_MinButtonColor", minbuttonColor.SelectedBrush);
            useSkin.MinButtonColor = minbuttonColor.Text;
        }

        private void MinbuttonFocusColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_MinButtonFocusColor");
            this.Resources.Add("_MinButtonFocusColor", minbuttonFocusColor.SelectedBrush);
            useSkin.MinButtonFocusColor = minbuttonFocusColor.Text;
        }

        private void MaxbuttonColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_MaxButtonColor");
            this.Resources.Add("_MaxButtonColor", maxbuttonColor.SelectedBrush);
            useSkin.MaxButtonColor = maxbuttonColor.Text;
        }

        private void MaxbuttonFocusColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_MaxButtonFocusColor");
            this.Resources.Add("_MaxButtonFocusColor", maxbuttonFocusColor.SelectedBrush);
            useSkin.MaxButtonFocusColor = maxbuttonFocusColor.Text;
        }

        private void ClosebuttonColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_CloseButtonColor");
            this.Resources.Add("_CloseButtonColor", closebuttonColor.SelectedBrush);
            useSkin.CloseButtonColor = closebuttonColor.Text;
        }

        private void ClosebuttonFocusColor_SelectedBrushChanged(object sender, Panuon.UI.Silver.Core.SelectedBrushChangedEventArgs e)
        {
            this.Resources.Remove("_CloseButtonFocusColor");
            this.Resources.Add("_CloseButtonFocusColor", closebuttonFocusColor.SelectedBrush);
            useSkin.CloseButtonFocusColor = closebuttonFocusColor.Text;
        }

        #endregion


        private void BtnCreateSkin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSkinName.Text))
            {
                MessageBox.Show("请输入主题名称");
                txtSkinName.Focus();
                return;
            }
            if (LocalSkin.Exists(txtSkinName.Text))
            {
                MessageBox.Show("当前主题名称已存在,请更改");
                txtSkinName.Focus();
                return;
            }

            useSkin.SkinName = txtSkinName.Text;
            LocalSkin.Insert(useSkin);
            Notice.Show($"创建[{useSkin.SkinName}]主题成功", "提示", 3, MessageBoxIcon.Success);

            BindMainComobox();
        }

        private void BtnSaveCurrSkin_Click(object sender, RoutedEventArgs e)
        {
            useSkin.SkinName = txtSkinName.Text;
            useSkin.SkinColor = cpSkinColor.Text;
            useSkin.SkinOppositeColor = cpSkinOppositeColor.Text;
            useSkin.PageBgColor = cpLoginBgColor.Text;

            //button
            useSkin.ButtonBgColor = buttonBgColor.Text;
            useSkin.ButtonBorderColor = buttonBorderColor.Text;
            useSkin.ButtonHoverColor = buttonHoverColor.Text;
            //textbox
            useSkin.TextBoxBgColor = textboxBgColor.Text;
            useSkin.TextBoxFocusColor = textboxFocusColor.Text;
            useSkin.TextBoxFocusedShadowColor = textboxFocusedShadowColor.Text;
            //datagrid
            useSkin.DataGridSelectedColor = datagridSelectedColor.Text;
            useSkin.DataGridHoverColor = datagridHoverColor.Text;
            useSkin.DataGridRowColor = datagridRowColor.Text;
            useSkin.DataGridEnableRowColor = datagridEnableRowColor.Text;
            //comobox
            useSkin.ComboBoxBgColor = comoboxBgColor.Text;
            useSkin.ComboBoxSelectedColor = comoboxSelectedColor.Text;
            useSkin.ComboBoxHoverColor = comoboxHoverColor.Text;
            //checkbox
            useSkin.CheckBoxBgColor = checkboxBgColor.Text;
            useSkin.CheckBoxSelectedColor = checkboxSelectedColor.Text;
            //treeview
            useSkin.TreeViewSelectedBgColor = treeviewSelectedBgColor.Text;
            useSkin.TreeViewSelectedHeaderColor = treeviewSelectedHeaderColor.Text;
            //pager
            useSkin.PagerBgColor = pagerBgColor.Text;
            useSkin.PagerHoverColor = pagerHoveColor.Text;
            //progressbar
            useSkin.ProgressBarBgColor = progressbarBgColor.Text;
            useSkin.ProgressBarFgColor = progressbarFgColor.Text;
            //system
            useSkin.MinButtonColor = minbuttonColor.Text;
            useSkin.MinButtonFocusColor = minbuttonFocusColor.Text;
            useSkin.MaxButtonColor = maxbuttonColor.Text;
            useSkin.MaxButtonFocusColor = maxbuttonFocusColor.Text;
            useSkin.CloseButtonColor = closebuttonColor.Text;
            useSkin.CloseButtonFocusColor = closebuttonFocusColor.Text;

            LocalSkin.Update(useSkin);
            MessageBox.Show("修改当前主题成功");

            //更新主题
            StyleHelper.UpdateSkin();
        }

        /// <summary>
        /// 修改编辑页面
        /// </summary>
        private void UpdateEditPage()
        {
            useSkin = LocalSkin.GetModelById(LocalSettings.settings.SkinId);
            if (useSkin != null && useSkin.SkinId > 0)
            {
                txtSkinName.Text = useSkin.SkinName;
                cpSkinColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.SkinColor);
                cpSkinOppositeColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.SkinOppositeColor);
                cpLoginBgColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.PageBgColor);

                //button
                buttonBgColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.ButtonBgColor);
                buttonBorderColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.ButtonBorderColor);
                buttonHoverColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.ButtonHoverColor);
                //textbox
                textboxBgColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.TextBoxBgColor);
                textboxFocusColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.TextBoxFocusColor);
                textboxFocusedShadowColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.TextBoxFocusedShadowColor);
                //datagrid
                datagridSelectedColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.DataGridSelectedColor);
                datagridHoverColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.DataGridHoverColor);
                datagridRowColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.DataGridRowColor);
                datagridEnableRowColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.DataGridEnableRowColor);
                //comobox
                comoboxBgColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.ComboBoxBgColor);
                comoboxSelectedColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.ComboBoxSelectedColor);
                comoboxHoverColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.ComboBoxHoverColor);
                //checkbox
                checkboxBgColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.CheckBoxBgColor);
                checkboxSelectedColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.CheckBoxSelectedColor);
                //treeview
                treeviewSelectedBgColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.TreeViewSelectedBgColor);
                treeviewSelectedHeaderColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.TreeViewSelectedHeaderColor);
                //pager
                pagerBgColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.PagerBgColor);
                pagerHoveColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.PagerHoverColor);
                //progressbar
                progressbarBgColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.ProgressBarBgColor);
                progressbarFgColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.ProgressBarFgColor);
                //system
                minbuttonColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.MinButtonColor);
                minbuttonFocusColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.MinButtonFocusColor);
                maxbuttonColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.MaxButtonColor);
                maxbuttonFocusColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.MaxButtonFocusColor);
                closebuttonColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.CloseButtonColor);
                closebuttonFocusColor.SelectedBrush = StyleHelper.ConvertToSolidColorBrush(useSkin.CloseButtonFocusColor);

            }
        }

    }
}
