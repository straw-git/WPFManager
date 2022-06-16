using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPlugins.Models
{
    /// <summary>
    /// 插件信息
    /// </summary>
    public class PluginsModel
    {
        /// <summary>
        /// 插件Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 插件中的必要文件数量
        /// </summary>
        public int FileCount { get; set; }
    }
}
