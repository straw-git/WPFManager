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
        /// 所有dll
        /// </summary>
        public static List<string> dllPaths;
        /// <summary>
        /// 插件所在文件夹路径
        /// </summary>
        public static string pluginsFolderPath = $"{AppDomain.CurrentDomain.BaseDirectory}plugins/";

        static AutoUpdatePlugins()
        {
            //构造
            dllPaths = new List<string>()
            {
                @"F:\源代码\472\CustomerPlugin\bin\Debug\CustomerPlugin.dll",
                @"F:\源代码\472\ERPPlugin\bin\Debug\ERPPlugin.dll",
                @"F:\源代码\472\FinancePlugin\bin\Debug\FinancePlugin.dll",
                @"F:\源代码\472\FixedAssetsPlugin\bin\Debug\FixedAssetsPlugin.dll",
                @"F:\源代码\472\HRPlugin\bin\Debug\HRPlugin.dll",
                @"F:\源代码\472\SalePlugin\bin\Debug\SalePlugin.dll"
            };
        }

        public static void Update()
        {
            if (!enable) return;

            foreach (var path in dllPaths)
            {
                //将当前路径文件复制到plugin文件夹下
                if (File.Exists(path)) //是否存在文件
                {
                    string fileName = path.Substring(path.LastIndexOf('\\') + 1);
                    string newPath = $"{pluginsFolderPath}{fileName}";

                    if (!isValidFileContent(path, newPath)) 
                    {
                        //将文件复制到插件文件夹
                        File.Copy(path, newPath, true);
                    }
                }
            }
        }

        /// <summary>
        /// 判断两个文件是否相同
        /// </summary>
        /// <param name="filePath1"></param>
        /// <param name="filePath2"></param>
        /// <returns></returns>
        public static bool isValidFileContent(string filePath1, string filePath2)
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
