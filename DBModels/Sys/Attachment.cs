using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels.Sys
{
    /// <summary>
    /// 附件表
    /// </summary>
    public class Attachment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }//附件名称
        public int FromTable { get; set; }//从哪个表来 Common.dll => AttachmentGlobal.TableNames
        public int DataId { get; set; }//对应数据表中的Id值
        [MaxLength(200)]
        public string SavedPath { get; set; }//保存的文件路径
        [MaxLength(100)]
        public string SavedName { get; set; }//保存的文件名

        public bool IsDel { get; set; }//是否删除
        public int DelUser { get; set; }//删除人
        public DateTime DelTime { get; set; }//删除时间

        public int Creator { get; set; }//创建人编号
        public string CreatorName { get; set; }//创建人名称
        public DateTime CreateTime { get; set; }//创建时间
    }
}
