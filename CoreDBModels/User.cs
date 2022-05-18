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

        public int RoleId { get; set; }//系统角色

        public int DeparmentId { get; set; }//所属部门
        public int DepartmentPositionId { get; set; }//部门职位Id
        public DateTime PositionEndTime { get; set; }//职位到期时间
        public int NewPositionId { get; set; }//到期后是否有新的职位
        public DateTime NewPositionStartTime { get; set; }//新职位开始时间
        public int PositionType { get; set; }//职位依据 0：合同 1：...

        public string RealName { get; set; }//真实姓名
        public string IdCard { get; set; }//身份证号

        public bool CanLogin { get; set; }//是否允许登录

        //删除状态
        public bool IsDel { get; set; }
        public int DelUser { get; set; }
        public DateTime DelTime { get; set; }
        //创建状态
        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }//入职时间
    }
}
