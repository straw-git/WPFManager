using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 插件模块
    /// </summary>
    public class PluginsModule
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PluginsId { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        [MaxLength(50)]
        public string ModuleName { get; set; }
        [MaxLength(20)]
        public string Icon { get; set; }
        public int Order { get; set; }

        [NotMapped]
        public string DLLName { get; set; }
    }
}
