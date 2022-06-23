using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 插件
    /// </summary>
    public class Plugins
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string DLLName { get; set; }//插件入口程序
        [MaxLength(1000)]
        public string DLLs { get; set; }//插件关联的DLL以|分隔
        [MaxLength(200)]
        public string LogoImage { get; set; }//Logo图的名称 图片在项目中
        public bool WebDownload { get; set; }//是否从网络下载
        public int Order { get; set; }//排序
        [MaxLength(100)]
        public string ConnectionName { get; set; }//连接字符串名称
        [MaxLength(1000)]
        public string ConnectionString { get; set; }//连接字符串
        [MaxLength(500)]
        public string DependentIds { get; set; }//依赖的插件Id 以|分隔

        public bool IsEnable { get; set; }//是否启用

        public DateTime UpdateTime { get; set; }//最后更新时间
    }
}
