using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Pwd { get; set; }

        public int RoleId { get; set; }//角色

        public bool CanLogin { get; set; }//是否允许登录

        //删除状态
        public bool IsDel { get; set; }
        public int DelUser { get; set; }
        public DateTime DelTime { get; set; }
        //创建状态
        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
