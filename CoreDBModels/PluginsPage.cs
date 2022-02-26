using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 插件中的页面
    /// </summary>
    public class PluginsPage
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PluginsId { get; set; }//插件Id
        [MaxLength(50)]
        public string ModuleTitle { get; set; }//模块标题
        [MaxLength(100)]
        public string ModuleFolderPath { get; set; }//模块文件夹
        [MaxLength(50)]
        public string PageTitle { get; set; }//页面标题
        [MaxLength(100)]
        public string PagePath { get; set; }//页面路径
    }
}
