
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRDBModels.Models
{
    /// <summary>
    /// 其它工资 扣款或奖励
    /// </summary>
    public class StaffSalaryOther
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string MonthCode { get; set; }//月度账号 2108
        [MaxLength(50)]
        public string StaffId { get; set; }
        public int Type { get; set; } //0:扣款 1：奖励
        public decimal Price { get; set; }//金额
        public DateTime DoTime { get; set; }//时间
        [MaxLength(200)]
        public string Remark { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
