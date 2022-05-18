using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 部门
    /// </summary>
    public class Department
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }//部门名称
        public int ParentId { get; set; }//上级部门id 0为一级部门

        [MaxLength(200)]
        public string Remark { get; set; }//部门备注信息

        public int Index { get; set; }//排序

        public bool IsDel { get; set; }//是否删除
        public int DelUser { get; set; }//执行删除的用户
        [MaxLength(200)]
        public string DelRemark { get; set; }//删除的备注

        public int Creator { get; set; }//创建人
        public DateTime CreateTime { get; set; }//创建时间
    }
}
