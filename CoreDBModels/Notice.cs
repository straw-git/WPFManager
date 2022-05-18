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

        public string Content { get; set; }//内容

        public string DepartmentName { get; set; }//发表部门
        public string UserName { get; set; }//发表人名称

        public DateTime CreateTime { get; set; }//创建时间
        public DateTime EndTime { get; set; }//结束时间
        public int Creator { get; set; }//创建人
    }
}
