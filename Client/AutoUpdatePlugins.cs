using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// 自动更新DLL插件（仅用于开发时自动更新其它插件更改）
    /// </summary>
    public class AutoUpdatePlugins
    {
        /// <summary>
        /// 自动更新
        /// </summary>
        public static bool enable = true;
        /// <summary>
        /// 所有插件dll
        /// </summary>
        public static List<string> pluginDLLPaths;
        /// <summary>
        /// 所有数据dll
        /// </summary>
        public static List<string> DBDLLPaths;
        /// <summary>
        /// 插件所在文件夹路径
        /// </summary>
        public static string pluginsFolderPath = $"{AppDomain.CurrentDomain.BaseDirectory}plugins/";

        static AutoUpdatePlugins()
        {
            //获取项目根目录
            string basePath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("Client"));

            //需要检查更新的窗体DLL
            pluginDLLPaths = new List<string>()
            {
                $@"{basePath}CustomerPlugin\bin\Debug\CustomerPlugin.dll",
                $@"{basePath}ERPPlugin\bin\Debug\ERPPlugin.dll",
                $@"{basePath}FinancePlugin\bin\Debug\FinancePlugin.dll",
                $@"{basePath}FixedAssetsPlugin\bin\Debug\FixedAssetsPlugin.dll",
                $@"{basePath}HRPlugin\bin\Debug\HRPlugin.dll",
                $@"{basePath}SalePlugin\bin\Debug\SalePlugin.dll",
                $@"{basePath}LiveChartsTestPlugin\bin\Debug\LiveChartsTestPlugin.dll",
                $@"{basePath}CorePlugin\bin\Debug\CorePlugin.dll"
            };
            //需要检查更新的数据实体 DLL
            DBDLLPaths = new List<string>()
            {
                $@"{basePath}FixedAssetsDBModels\bin\Debug\FixedAssetsDBModels.dll",
                //$@"{basePath}CoreDBModels\bin\Debug\CoreDBModels.dll", 这个是必须要连接的
                $@"{basePath}ERPDBModels\bin\Debug\ERPDBModels.dll",
                $@"{basePath}CustomerDBModels\bin\Debug\CustomerDBModels.dll",
                $@"{basePath}HRDBModels\bin\Debug\HRDBModels.dll",
                $@"{basePath}SaleDBModels\bin\Debug\SaleDBModels.dll",
                $@"{basePath}FinanceDBModels\bin\Debug\FinanceDBModels.dll",
            };
        }

        public static void Update()
        {
            if (!enable) return;

            #region 核对窗体DLL

            foreach (var path in pluginDLLPaths)
            {
                //将当前路径文件复制到plugin文件夹下
                if (File.Exists(path)) //是否存在文件
                {
                    string fileName = path.Substring(path.LastIndexOf('\\') + 1);
                    string newPath = $"{pluginsFolderPath}{fileName}";

                    if (!File.Exists(newPath))
                    {
                        //如果在插件列表中没有存在这个插件 直接复制进来
                        File.Copy(path, newPath, true);
                        continue;
                    }

                    if (!IsValidFileContent(path, newPath))
                    {
                        //将文件复制到插件文件夹
                        File.Copy(path, newPath, true);
                    }
                }
            }

            #endregion

            #region 核对 DBModels

            foreach (var path in DBDLLPaths)
            {
                //将当前路径文件复制到plugin文件夹下
                if (File.Exists(path)) //是否存在文件
                {
                    string fileName = path.Substring(path.LastIndexOf('\\') + 1);
                    string newPath = $"{AppDomain.CurrentDomain.BaseDirectory}{fileName}";

                    if (!File.Exists(newPath))
                    {
                        //如果在插件列表中没有存在这个插件 直接复制进来
                        File.Copy(path, newPath, true);
                        continue;
                    }

                    if (!IsValidFileContent(path, newPath))
                    {
                        //将文件复制到插件文件夹
                        File.Copy(path, newPath, true);
                    }
                }
            }

            #endregion
        }

        public static async void UpdateAsync()
        {
            await Task.Run(() =>
            {
                Update();
            });
        }

        /// <summary>
        /// 判断两个文件是否相同
        /// </summary>
        /// <param name="filePath1"></param>
        /// <param name="filePath2"></param>
        /// <returns></returns>
        public static bool IsValidFileContent(string filePath1, string filePath2)
        {
            //创建一个哈希算法对象
            using (HashAlgorithm hash = HashAlgorithm.Create())
            {
                using (FileStream file1 = new FileStream(filePath1, FileMode.Open), file2 = new FileStream(filePath2, FileMode.Open))
                {
                    byte[] hashByte1 = hash.ComputeHash(file1);//哈希算法根据文本得到哈希码的字节数组
                    byte[] hashByte2 = hash.ComputeHash(file2);
                    string str1 = BitConverter.ToString(hashByte1);//将字节数组装换为字符串
                    string str2 = BitConverter.ToString(hashByte2);
                    return (str1 == str2);//比较哈希码
                }
            }
        }
    }
}
