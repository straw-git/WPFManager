using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 基础设置
    /// </summary>
    public class CoreSetting
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MaxLogCount { get; set; }

        [MaxLength(500)]
        public string PluginsUpdateBaseUrl { get; set; }//插件更新的基础路径
    }
}
