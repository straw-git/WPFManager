using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CoreDBModels
{
    /// <summary>
    /// 邮件发送信息
    /// </summary>
    public class EmailSendTo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmailId { get; set; }//邮件
        public string EmailTitle { get; set; }//邮件标题
        public int UserId { get; set; }//用户Id    ====UserId 与 RoleId 同时=0的时候 就是发送给所有人的
        public int RoleId { get; set; }//角色Id
        public bool IsRead { get; set; }//是否阅读
        public string UserReadTime { get; set; }//阅读时间（只有UserId>0时 使用）
        public DateTime SendTime { get; set; }//发送时间
    }
}
