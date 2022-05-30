using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 邮件
    /// </summary>
    public class Email
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FromId { get; set; }//发送用户Id
        public int EmailType { get; set; }//通知类型 0：按用户 1：按角色  2：全部
        public string Content { get; set; }//内容
        public string Title { get; set; }//标题
        public DateTime SendTime { get; set; }//发送时间
    }
}
