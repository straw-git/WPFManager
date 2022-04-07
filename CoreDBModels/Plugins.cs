﻿using System;
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
        public string DLLName { get; set; }
        [MaxLength(200)]
        public string LogoImage { get; set; }//Logo图的名称 图片在项目中
        public bool WebDownload { get; set; }//是否从网络下载
        public int Order { get; set; }//排序
        public string ConnectionName { get; set; }//连接字符串名称
        public string ConnectionString { get; set; }//连接字符串

        public DateTime UpdateTime { get; set; }//最后更新时间
    }
}
