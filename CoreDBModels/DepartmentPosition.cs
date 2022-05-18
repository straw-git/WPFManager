using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 部门职位
    /// </summary>
    public class DepartmentPosition
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }//职位名称
        public int DepartmentId { get; set; }//所属部门

        [MaxLength(200)]
        public string Remark { get; set; }//职位备注信息

        public int Index { get; set; }//排序

        public bool IsDel { get; set; }//是否删除
        public int DelUser { get; set; }//执行删除的用户
        [MaxLength(200)]
        public string DelRemark { get; set; }//删除的备注

        public int MaxUserCount { get; set; }//最大使用用户数

        public int Creator { get; set; }//创建人
        public DateTime CreateTime { get; set; }//创建时间
    }
}
