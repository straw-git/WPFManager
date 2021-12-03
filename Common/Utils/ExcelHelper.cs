using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using form = System.Windows.Forms;

namespace Common.Utils
{
    public class ExcelHelper
    {
        /// <summary>
        /// 将List导出至Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_list">集合</param>
        /// <param name="_sheetName"></param>
        /// <param name="_colums">列和标题对应</param>
        /// <param name="_hiddleColumns">隐藏列名</param>
        public async void List2ExcelAsync<T>(List<T> _list, string _sheetName = "导出文件_Zyue", Dictionary<string, string> _colums = null, List<string> _hiddleColumns = null)
        {
            if (_colums == null || _colums.Count == 0)
            {
                MessageBoxX.Show("没有显示列数据","空值提醒");
                return;
            }

            //Panuon.UI.Silver消息框
            var _handler = PendingBox.Show("正在导出Excel...", "请等待", false, null, new PendingBoxConfigurations()
            {
                LoadingForeground = "#5DBBEC".ToColor().ToBrush(),
                ButtonBrush = "#5DBBEC".ToColor().ToBrush(),
            });

            string _savePath = "";//文件保存路径
            var folderBrowser = new form.FolderBrowserDialog();//Winform dll中调用选择文件夹
            if (folderBrowser.ShowDialog() == form.DialogResult.OK)
            {
                _savePath = folderBrowser.SelectedPath;//选中的文件夹路径
            }
            else
            {
                MessageBox.Show("已取消");
                _handler.Close();
            }

            string excelFullPath = $"{_savePath}\\[{_sheetName}]{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";//导出文件路径

            await Task.Run(() =>
            {
                IWorkbook wk = null;
                wk = new XSSFWorkbook();//创建一个工作簿

                var sheet = wk.CreateSheet(_sheetName);//创建Sheet

                if (_list != null && _list.Count > 0)
                {
                    #region 添加第一行标题数据

                    int propertiesCount = 0;//属性

                    foreach (var key in _colums.Keys)
                    {
                        //属性名称
                        string name = key;
                        string title = _colums[key];

                        if (_hiddleColumns != null)
                        {
                            if (_hiddleColumns.Any(c => c == name))
                            {
                                continue;
                            }
                        }

                        var _row = propertiesCount == 0 ? sheet.CreateRow(0) : sheet.GetRow(0);//创建或获取第一行

                        var _headerCell = _row.CreateCell(propertiesCount);//创建列
                        _headerCell.SetCellValue(name);//设置列值
                        propertiesCount++;
                    }

                    #endregion

                    var _headerRow = sheet.GetRow(0);//获取第一行

                    #region 添加数据

                    for (int i = 0; i < _list.Count; i++)
                    {
                        var _item = _list[i];

                        var _currRow = sheet.CreateRow(1 + i);//创建当前数据的行
                        for (int j = 0; j < propertiesCount; j++)
                        {
                            string _propertityName = _headerRow.GetCell(j).StringCellValue;//获取属性名称
                            if (_hiddleColumns != null)
                            {
                                if (_hiddleColumns.Any(c => c == _propertityName))
                                {
                                    continue;
                                }
                            }
                            string _propertityValue = _item.GetType().GetProperty(_propertityName).GetValue(_item, null).ToString();//获取属性值

                            _currRow.CreateCell(j).SetCellValue(_propertityValue);//设置单元格数据
                        }
                    }

                    #endregion

                    #region 更新列标题

                    for (int j = 0; j < propertiesCount; j++)
                    {
                        string _propertityName = _headerRow.GetCell(j).StringCellValue;//获取属性名称
                        if (_colums != null && _colums.ContainsKey(_propertityName)) _headerRow.CreateCell(j).SetCellValue(_colums[_propertityName]);//更新标题
                    }

                    #endregion 
                }
                else
                {
                    UIGlobal.RunUIAction(() =>  //其它县城内调用UI主线程方法
                    {
                        MessageBox.Show("没有导出数据");
                        _handler.Close();
                    });
                    return;
                }

                try
                {
                    using (FileStream fs = new FileStream(excelFullPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                    {
                        wk.Write(fs);//保存文件
                    }
                }
                catch (Exception ex)
                {
                    UIGlobal.RunUIAction(() =>
                    {
                        MessageBoxX.Show($"导出失败, [{ex.Message}]", "文件占用提示");
                        _handler.Close();
                    });
                }
            });

            Notice.Show(excelFullPath, "Excel导出成功", 3, MessageBoxIcon.Success);
            _handler.Close();
        }
    }
}
