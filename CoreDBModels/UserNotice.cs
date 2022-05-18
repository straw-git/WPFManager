using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 用户通知
    /// </summary>
    public class UserNotice
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }//用户
        public int NoticeId { get; set; }//通知
        public bool IsRead { get; set; }//是否阅读
        public DateTime ReadTime { get; set; }//阅读时间
    }
}
