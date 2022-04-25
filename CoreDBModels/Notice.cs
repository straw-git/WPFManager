using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 通知
    /// </summary>
    public class Notice
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int NoticeType { get; set; }//通知类型 0：全员 1：单独用户  2：单独角色
        public string Content { get; set; }//内容

        public DateTime StartTime { get; set; }//开始时间
        public DateTime EndTime { get; set; }//结束时间
    }
}
