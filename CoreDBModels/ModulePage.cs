using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 插件中的页面
    /// </summary>
    public class ModulePage
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PluginsId { get; set; }//插件Id
        public int ModuleId { get; set; }//模块Id
        [MaxLength(50)]
        public string PageName { get; set; }//页面名称
        [MaxLength(500)]
        public string PagePath { get; set; }//页面路径
        [MaxLength(20)]
        public string Icon { get; set; }
        public int Order { get; set; }//排序

        [NotMapped]
        public string FullPath { get; set; }
    }
}
