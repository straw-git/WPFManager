using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels.Models
{
    public class SysDic
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string QuickCode { get; set; } // 简码 、快速编码 用于检索 一般为英文拼音检索
        [MaxLength(100)]
        public string ParentCode { get; set; }
        [MaxLength(500)]
        public string Content { get; set; }

        public int Creater { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
