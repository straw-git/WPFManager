using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRDBModels
{
    /// <summary>
    /// 员工保险
    /// </summary>
    public class StaffInsurance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Type { get; set; } //保险类型  0：社保  1：商业保险

        public DateTime Start { get; set; }//开始时间

        public bool Monthly { get; set; } //是否按月扣款
        public bool Stop { get; set; }//是否主动停保 Monthly==true的可以主动停保
        public DateTime End { get; set; }//截止时间 Monthly==true的 结束时间为停保时间
        public DateTime Write { get; set; }//签订时间

        public decimal CompanyPrice { get; set; }//单位承担金额 Monthly==true的为月度金额 false为总金额
        public decimal StaffPrice { get; set; }//员工承担金额 Monthly==true的为月度金额 false为总金额

        [MaxLength(200)]
        public string CompanyName { get; set; }//办理单位

        [MaxLength(200)]
        public string Remark { get; set; }

        [MaxLength(50)]
        public string StaffId { get; set; }

        public int StopUser { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
