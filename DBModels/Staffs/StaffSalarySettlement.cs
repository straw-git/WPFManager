

using System;
using System.ComponentModel.DataAnnotations;

namespace DBModels.Staffs
{
    /// <summary>
    /// 工资结算
    /// </summary>
    public class StaffSalarySettlement
    {
        [Key]
        [MaxLength(10)]
        public string Id { get; set; }//月度账号 2108
        public int StaffCount { get; set; }//结算人数
        public decimal Price { get; set; }//工资总额

        public int Creator { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
