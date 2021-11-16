using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class LocalFileHelper
    {
        string filePath = "";

        public enum LocalFileDicType
        {
            /// <summary>
            /// bin目录下
            /// </summary>
            LocalBin,
            /// <summary>
            /// bin/data目录下
            /// </summary>
            LocalData,
            LocalSourceCode,
            /// <summary>
            /// 其他目录
            /// </summary>
            Other
        }

        public LocalFileHelper(LocalFileDicType _dicType = LocalFileDicType.Other, string _baseFolderName = "")
        {
            switch (_dicType)
            {
                case LocalFileDicType.LocalBin:
                    filePath = string.IsNullOrEmpty(_baseFolderName) ? AppDomain.CurrentDomain.BaseDirectory : $"{AppDomain.CurrentDomain.BaseDirectory}{_baseFolderName}";
                    break;
                case LocalFileDicType.LocalData:
                    filePath = string.IsNullOrEmpty(_baseFolderName) ? $"{AppDomain.CurrentDomain.BaseDirectory}data" : $"{AppDomain.CurrentDomain.BaseDirectory}data\\{_baseFolderName}";
                    break;
                case LocalFileDicType.LocalSourceCode:
                    filePath = string.IsNullOrEmpty(_baseFolderName) ? $"{AppDomain.CurrentDomain.BaseDirectory}data\\SourcesCode" : $"{AppDomain.CurrentDomain.BaseDirectory}data\\SourcesCode\\{_baseFolderName}";
                    break;
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <returns></returns>
        public string ReadFile(string _fileName)
        {
            return ReadFile(filePath, "", _fileName);
        }

        public string ReadFileByPath(string _file)
        {
            if (!File.Exists(_file))
            {
                //return $"[{_file}] 文件不存在";
                return "";
            }

            return File.ReadAllText(_file, Encoding.UTF8);
        }

        public string ReadFile(string _filePath, string _folderName, string _fileName)
        {
            if (!string.IsNullOrEmpty(_folderName))
            {
                if (_filePath.EndsWith("\\"))
                {
                    _filePath = $"{_filePath}{_folderName}";
                }
                else
                {
                    _filePath = $"{_filePath}\\{_folderName}";
                }
            }
            if (!Directory.Exists(_filePath))
            {
                //return $"[{_filePath}] 路径不存在";
                return "";
            }
            _filePath = $"{_filePath}\\{_fileName}";

            return ReadFileByPath(_filePath);
        }

        public void SaveNew(string _fileName, string _content)
        {
            SaveNew(filePath, "", _fileName, _content);
        }

        public void SaveNew(string _filePath, string _folderName, string _fileName, string _content)
        {
            if (!string.IsNullOrEmpty(_folderName))
            {
                if (_filePath.EndsWith("\\"))
                {
                    _filePath = $"{_filePath}{_folderName}";
                }
                else
                {
                    _filePath = $"{_filePath}\\{_folderName}";
                }
            }

            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }

            _filePath = $"{_filePath}\\{_fileName}";


            if (!File.Exists(_filePath))
            {
                var s = File.Create(_filePath);
                s.Close();
                s.Dispose();
            }

            //json数据处理
            if (!string.IsNullOrEmpty(_content))
            {
                //写入文件
                File.WriteAllText(_filePath, _content);
            }
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="_folderName"></param>
        /// <param name="_jsonFileName"></param>
        public void DeleteFile(string _fileName, string _fileType = ".txt")
        {
            string _filePath = $"{filePath}\\{_fileName}{_fileType}";
            try
            {
                File.Delete(_filePath);
            }
            catch
            {
                //可能是路径不存在
            }
        }

    }
