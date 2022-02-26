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
        public string Title { get; set; }//插件显示的标题
        [MaxLength(100)]
        public string DLLName { get; set; }//插件的DLL名称
        [MaxLength(300)]
        public string ModuleTitles { get; set; }//模块名称 以，分隔
        [MaxLength(500)]
        public string ModuleFolderPaths { get; set; }//模块对应的文件夹 以，分隔 
        [MaxLength(300)]
        public string ModuleIcon { get; set; }//模块对应的Icon图标或图片 以，分隔
        [MaxLength(200)]
        public string UpdateUrl { get; set; }//插件更新路径
        [MaxLength(500)]
        public string DBModelUrl { get; set; }//对应的数据实体dll文件 以，分隔
        public int Order { get; set; }//排序

        public DateTime UpdateTime { get; set; }//最后更新时间
    }
}
