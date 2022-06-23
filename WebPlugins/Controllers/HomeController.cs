using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebPlugins.Models;

namespace WebPlugins.Controllers
{
    public class FormObject
    {
        public List<IFormFile> Files { get; set; }
        public string Name { get; set; }
    }
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            string sql = "select [id],[name],[dlls] from plugins";

            List<PluginsModel> list = new List<PluginsModel>();
            DataTable dt = DBHelper.Query(sql).Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PluginsModel model = new PluginsModel();
                    model.Id = (int)dt.Rows[i]["Id"];
                    model.Name = dt.Rows[i]["Name"].ToString();
                    string dlls = dt.Rows[i]["DLLs"].ToString();
                    model.FileCount = 0;
                    var dllArr = dlls.Split('|');
                    if (dllArr.Length > 0)
                    {
                        for (int j = 0; j < dllArr.Length; j++)
                        {
                            string dllName = dllArr[j];
                            if (dllName == string.Empty) continue;
                            model.FileCount += 1;
                        }
                    }

                    list.Add(model);
                }
            }

            ViewData["Plugins"] = list;

            return View();
        }

        public IActionResult Edit(int id)
        {
            string sql = @$"select [id],[name],[dlls] from plugins where id={id}";

            string pluginsName = "";

            List<PluginsDetailModel> list = new List<PluginsDetailModel>();
            DataTable dt = DBHelper.Query(sql).Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                pluginsName = dt.Rows[0]["Name"].ToString();
                string dlls = dt.Rows[0]["DLLs"].ToString();
                var dllArr = dlls.Split('|');
                if (dllArr.Length > 0)
                {
                    for (int j = 0; j < dllArr.Length; j++)
                    {
                        string dllName = dllArr[j];
                        if (dllName == string.Empty) continue;

                        PluginsDetailModel model = new PluginsDetailModel();
                        model.Name = dllName;

                        list.Add(model);
                    }
                }
            }
            ViewData["PluginsId"] = id;
            ViewData["PluginsName"] = pluginsName;
            ViewData["Details"] = list;
            return View();
        }

        [HttpPost]
        public void UploadDlls(int id)
        {
            var files = Request.Form.Files;//获取上传的文件

            string sql = @$"select [id],[name],[dlls] from plugins where id={id}";
            string pluginsDlls = "";
            DataTable dt = DBHelper.Query(sql).Tables[0];//获取数据库中的插件
            if (dt != null && dt.Rows.Count == 1)
            {
                pluginsDlls = dt.Rows[0]["DLLs"].ToString();//插件的辅助资料
            }

            foreach (var file in files)
            {
                var fileName = file.FileName;//文件名

                #region 将文件上传

                fileName = _webHostEnvironment.WebRootPath + $@"\dlls\{id}\{fileName}";
                if (!Directory.Exists(_webHostEnvironment.WebRootPath + $@"\dlls\{id}"))
                {
                    Directory.CreateDirectory(_webHostEnvironment.WebRootPath + $@"\dlls\{id}");
                }
                using (FileStream fs = System.IO.File.Create(fileName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }

                if (string.IsNullOrEmpty(pluginsDlls))
                {
                    pluginsDlls = file.FileName;
                }
                else
                {
                    if (!pluginsDlls.Contains(file.FileName))
                        pluginsDlls += $"|{file.FileName}";
                }

                #endregion
            }

            sql = $"update plugins set [dlls]='{pluginsDlls}' where id={id}";
            DBHelper.ExecuteSql(sql);//更改数据库
        }

        [HttpPost]
        public string PostFile([FromForm] IFormCollection formCollection)
        {
            string result = "Fail";
            FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
            foreach (IFormFile file in fileCollection)
            {
                StreamReader reader = new StreamReader(file.OpenReadStream());
                string content = reader.ReadToEnd();
                string suffix = file.FileName.Substring(file.FileName.LastIndexOf('.'));

                string fileName = "";
                string filePath = "";
                if (suffix == ".jpg" || suffix == ".png" || suffix == ".jpeg" || suffix == ".bmp" || suffix == ".gif")
                {
                    fileName = $"{Guid.NewGuid().ToString()}{suffix}";
                    filePath = _webHostEnvironment.WebRootPath + $@"\imgs\{fileName}";
                }
                else 
                {
                    fileName = file.FileName;
                    filePath = _webHostEnvironment.WebRootPath + $@"\dlls\{fileName}";
                }

               
                if (!System.IO.File.Exists(filePath))
                {
                    //存在文件 直接返回 防止文件重复
                    //System.IO.File.Delete(filePath);
                    //不存在 复制文件
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        // 复制文件
                        file.CopyTo(fs);
                        // 清空缓冲区数据
                        fs.Flush();
                    }
                }

                result = $"{fileName}";
            }
            return result;
        }

        /// <summary>
        /// 下载插件
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DownloadPlugins(int pluginId, string fileName)
        {
            string sFileName = _webHostEnvironment.WebRootPath + $@"\dlls\{pluginId}\{fileName}";
            FileStream fs = new FileStream(sFileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.Flush();
            sw.Close();
            fs.Close();
            return File(new FileStream(sFileName, FileMode.Open), "application/octet-stream", fileName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
